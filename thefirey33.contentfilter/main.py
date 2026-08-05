import asyncio
import logging

from better_fastapi_discord import DiscordOAuthClient
from transformers import pipeline
from fastapi import FastAPI, Response, status
from discord.ext.commands import Bot
from contextlib import  asynccontextmanager
from logging import Logger, getLogger, basicConfig
import os
import discord

basicConfig(level=logging.INFO)
logger = getLogger(__name__)

authenticationCommunicationService = DiscordOAuthClient(
    os.environ["CLIENT_ID"],
    os.environ["CLIENT_SECRET"],
    os.environ["REDIRECT_URI"]
)

# This is for detecting text, that are potentially OFFENSIVE.
offensive_speech_detection = pipeline("text-classification", model="Falconsai/offensive_speech_detection")

# This is for detecting images, that are potentially NSFW.
nsfw_image_detection = pipeline("image-classification", model="Falconsai/nsfw_image_detection")

bot = Bot(command_prefix="!", intents=discord.Intents.all())

@asynccontextmanager
async def lifespan(_: FastAPI):
    # Create the tasks for the bot and the OAuth2 service.

    loop = asyncio.get_event_loop()
    loop.create_task(bot.start(token=os.environ["BOT_TOKEN"]))
    await authenticationCommunicationService.init()

    if authenticationCommunicationService.client_session is None:
        raise Exception("No OAuth2 Client Session Available!")

    logger.info("OAuth2 Service Online! Session: %s", authenticationCommunicationService.client_session)
    yield
    # Close the remaining sessions if possible.

    await authenticationCommunicationService.client_session.close()
    await bot.close()
app = FastAPI(lifespan=lifespan)

@bot.event
async def on_ready():
    await bot.change_presence(status=discord.Status.online, activity=discord.Game(name="authenticating, mew~"))

@app.get("/health", tags=["health"])
def health():
    """
    This is for the Aspire health check.
    """
    return Response(status_code=status.HTTP_200_OK)

@app.get("/login", tags=["oauth"])
async def login():
    """
    Discord Authentication Redirect URL.
    """
    return {"url": authenticationCommunicationService.oauth_login_url}