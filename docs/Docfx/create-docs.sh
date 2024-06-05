#!/bin/bash

#Ensure that Docfx is installed
echo "Attempting install of Docfx..."
dotnet tool update -g docfx

# Create the Docfx metadata
echo "Running Docfx metadata..."
docfx metadata docfx.json

# Run Python script in the same directory as this bash script
echo "Running Python script..."
python3 ./your_script.py

# Finish building the site
echo "Running Docfx build..."
docfx build docfx.json


echo "Build Complete."
