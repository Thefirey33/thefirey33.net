import os

from better_fastapi_discord import DiscordOAuthClient, User, RateLimited, Unauthorized
from better_fastapi_discord.exceptions import ClientSessionNotInitialized
from fastapi import Depends, APIRouter
from starlette.responses import JSONResponse

# Make it an APIRouter otherwise the main.py file gets too cluttered.
router = APIRouter(
    prefix="/auth"
)

authenticationCommunicationService = DiscordOAuthClient(
    os.environ["CLIENT_ID"],
    os.environ["CLIENT_SECRET"],
    "",
    ("identify", "email"),
    proxy=os.environ["PROXY"]
)

@router.get("/callback", tags=["oauth2"])
async def callback(code: str):
    """
    If the Discord API is successful, it gives the access and refresh tokens.
    """
    token, refresh_token = await authenticationCommunicationService.get_access_token(code)
    return {"access_token": token, "refresh_token": refresh_token}


@router.get(
    "/authenticated",
    dependencies=[Depends(authenticationCommunicationService.requires_authorization)],
    response_model=bool, tags=["oauth2"]
)
async def is_authenticated(token: str = Depends(authenticationCommunicationService.get_token)):
    """
    Checks if the specified user is authenticated or not.
    """
    try:
        auth = await authenticationCommunicationService.isAuthenticated(token)
        return auth
    except Unauthorized:
        return False


async def unauthorized_error_handler(_, __):
    return JSONResponse({"error": "Unauthorized"}, status_code=401)


async def rate_limit_error_handler(_, e: RateLimited):
    return JSONResponse(
        {"error": "RateLimited", "retry": e.retry_after, "message": e.message},
        status_code=429,
    )


# noinspection unused-parameter
async def client_session_error_handler(_, e: ClientSessionNotInitialized):
    return JSONResponse({"error": "Internal Error"}, status_code=500)


@router.get("/user", tags=["discord_data"], dependencies=[Depends(authenticationCommunicationService.requires_authorization)], response_model=User)
async def get_user(user: User = Depends(authenticationCommunicationService.user)):
    """
    Get the current discord user.
    """
    return user

@router.get("/login", tags=["oauth2"])
async def login(redirect_uri: str):
    """
    Discord Authentication Redirect URL.
    """
    authenticationCommunicationService.redirect_uri = redirect_uri
    return {"url": authenticationCommunicationService.oauth_login_url}