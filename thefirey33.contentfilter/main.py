import asyncio
import io
import logging
from typing import Annotated

from PIL import Image, UnidentifiedImageError
from better_fastapi_discord import RateLimited, Unauthorized
from better_fastapi_discord.exceptions import ClientSessionNotInitialized
from fastapi import FastAPI, Response, status, Depends, UploadFile, Form, HTTPException, File
from discord.ext.commands import Bot
from contextlib import asynccontextmanager
from logging import getLogger, basicConfig
import os
import discord
import authorization


class OAuth2SessionInvalid(Exception):
    pass

class ChannelSpecificationError(Exception):
    pass

basicConfig(level=logging.INFO)
logger = getLogger(__name__)

bot = Bot(command_prefix="!", intents=discord.Intents.all(), proxy=os.getenv("PROXY"))

@asynccontextmanager
async def lifespan(_: FastAPI):
    """
    Lifespan manager for the bot.
    """
    # Create the tasks for the bot and the OAuth2 service.

    loop = asyncio.get_event_loop()
    loop.create_task(bot.start(token=os.environ["BOT_TOKEN"]))
    await authorization.authenticationCommunicationService.init()

    if authorization.authenticationCommunicationService.client_session is None:
        raise OAuth2SessionInvalid("No OAuth2 Client Session Available!")

    logger.info("OAuth2 Service Online! Session: %s", authorization.authenticationCommunicationService.client_session)
    yield

    # Close the remaining sessions if possible.

    await authorization.authenticationCommunicationService.client_session.close()
    await bot.close()

app = FastAPI(lifespan=lifespan)

# Include the authentication router.
app.include_router(authorization.router)

# Add all the exception handlers for the authorization service.
app.add_exception_handler(RateLimited, authorization.rate_limit_error_handler)
app.add_exception_handler(Unauthorized, authorization.unauthorized_error_handler)
app.add_exception_handler(ClientSessionNotInitialized, authorization.client_session_error_handler)

@bot.event
async def on_ready():
    await bot.change_presence(status=discord.Status.do_not_disturb, activity=discord.Game(name="authenticating, mew~"))

@app.get("/health", tags=["health"])
def health():
    """
    This is for the Aspire health check.
    """
    return Response(status_code=status.HTTP_200_OK)

@app.post("/blogpost")
async def send_blog_announcement(id: int):
    """This sends an announcement about blog posts.

    Args:
        id (int): The ID of the blog post.

    Raises:
        ChannelSpecificationError: Thrown if the Channel isn't specified correctly.
    """
    channel = await bot.fetch_channel(int(os.getenv("CHANNEL_ID")))
    
    if not channel:
        raise ChannelSpecificationError("Channel wasn't found!")
    
    await channel.send(f"<@&{os.getenv("ROLE_ID")}> {os.getenv("BLOG_ORIGIN")}{id}")
    return Response(status_code=status.HTTP_200_OK)