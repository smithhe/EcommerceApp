---
_layout: landing
---

# Ecommerce Docs
Welcome to the docs for the Ecommerce App.

## Project Purpose:
The original purpose of this project was to provide a full stack web application for learning purposes. Most tutorials that are available online allow you to test with a greenfield project but never really show how you would implement the tool/framework you are learning into an existing project. Additionally other things like auth tutorials would always avoid following best practices for the sake of keeping the demo within the scope of the topic being covered.

Wanting to get away from these problems and also have a tool that I can work to practice maintaining as well as test out new frameworks and tools on I have created the Ecommerce App. A simple but fairly decently sized application that operates as a web store for tech items such as laptops and phones simulating most of what you would expect in a very minimal online store.

After a few months of development it became apparent to me via conversations with others that this tool could be useful for others wanting to learn how to build a full stack application from scratch. Not only just the application, but the devops process, following security standards, being able to reproduce the same result on a fairly minimal budget, etc.

## Instructions
For instructions on how to get the project up and running locally click here

If you are interested in forking the project to try running it in your own environment a guide will be posted soon on how to do that.

## Project Description
The following documentation has been designed for those looking to get more in depth with the skills they have, while I have tried to make it fairly beginner friendly some of what you will find in this project may not be beginner friendly and I strongly encourage those who want to use this tool for learning to make sure they have foundational knowledge in the areas they wish to explore further.

The tech stack for this project is mainly .Net with Blazor as the frontend and .Net API using fast-endpoints as a backend. Additionally there is a console project that acts as a worker to handle messages put on a queue by the API, currently that is limited to sending emails out.

From a devops perspective the apps all get containerized via the docker files and are pushed to a dockerhub repo. I chose to use DigitalOcean for hosting my cloud environment given my previous history with them as well as the better pricing for hosting a sql server and a k8s environment. Terraform was used to setup most of the cloud resources but some manual work is needed, such as setting up DNS and ssl certificates, etc.

For more details please follow a link to the Introduction page.