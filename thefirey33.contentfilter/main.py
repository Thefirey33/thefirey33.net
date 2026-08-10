import asyncio
import io
import logging
from typing import Annotated

from PIL import Image, UnidentifiedImageError
from better_fastapi_discord import RateLimited, Unauthorized
from better_fastapi_discord.exceptions import ClientSessionNotInitialized
from transformers import pipeline
from fastapi import FastAPI, Response, status, Depends, UploadFile, Form, HTTPException, File
from discord.ext.commands import Bot
from contextlib import  asynccontextmanager
from logging import getLogger, basicConfig
import os
import discord
import authorization

basicConfig(level=logging.INFO)
logger = getLogger(__name__)

# This is for detecting images, that are potentially NSFW.
nsfw_image_detection = pipeline("image-classification", model="Falconsai/nsfw_image_detection")

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
        raise Exception("No OAuth2 Client Session Available!")

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

@app.post("/content_check", tags=["classifiers"])
async def content_check(description: str = Form(), file: UploadFile | None = File(None)):
    """
    This section checks for NSFW content using content classification.
    If it is NSFW, it will return True. Otherwise, it will return False.
    """
    logger.info("Scanning uploaded content for NSFW content...")
    unsafe = False

    if file:
        file_bytes = await file.read()
        try:
            image_data = Image.open(io.BytesIO(file_bytes))
        except UnidentifiedImageError:
            raise HTTPException(status.HTTP_400_BAD_REQUEST, detail="Bad Image Type!")

        image_detection_result = nsfw_image_detection(image_data)

        # Check if the specified image is NSFW, if it is, label it as UNSAFE.
        if image_detection_result[0]['label'] == 'nsfw':
            unsafe = True

    return unsafe