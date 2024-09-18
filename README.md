# HoloGpt
 
Hello,

I'd like to talk about my recent hobby project that I've been working on lately, HoloGPT 1.0.

HoloGPT is a project that takes commands via voice recognition, processes the command, generates a response, vocalizes the response, and models it in augmented reality in the real world.

Stages of the Project:

1. Speech to Text:

In the application, pressing the 'recorder' button records audio from the microphone, which is then converted to text using Unity. I utilized a public GitHub repository by Sercan (Sarge) Altundaş for this process, which was incredibly helpful. Many thanks for that.

GitHub repository: https://lnkd.in/dDwHNYBf

2. Sending Prompt and Receiving Response:

After the Speech to Text stage, we send the command using the ChatGPT 3.5 Turbo API service and display the response on the screen. For more details: https://lnkd.in/dK5NQJjf

3. Text to Speech:

At this stage, we take the response from the API request and vocalize it. I'm grateful to Martin P. for the Text to Speech stage.

Related GitHub Repository: https://lnkd.in/dUC58uXs

Following these stages, the voice command system in my project became operational. Next, I integrated a Weather API service to fetch real-time weather information for HoloGPT 1.0.

Weather API service used: https://lnkd.in/du4nUcM6

Using the 'weather' checkbox on the screen, we can specify whether to fetch weather data or not.

My favorite feature is that when giving voice commands about the weather in any city, the system recognizes the city and sends requests specifically for that city, incorporating the response into HoloGPT's vocal response.

In the next update, I will try to enhance HoloGPT further by integrating a different API to make it a better assistant.

hashtag#AI hashtag#ArtificialIntelligence hashtag#MachineLearning hashtag#ChatGPT hashtag#AugmentedReality hashtag#AR hashtag#Unity hashtag#SpeechRecognition hashtag#TextToSpeech hashtag#WeatherAPI hashtag#TechProjects hashtag#Programming hashtag#Coding hashtag#SoftwareDevelopment hashtag#Innovation hashtag#PersonalProjects hashtag#OpenAi hashtag#VoiceAssistant
