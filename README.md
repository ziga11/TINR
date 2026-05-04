# TINR
A simple 2D game inspired by the old game `Miscrits: World of Creatures` made in C# using the framework MonoGame.

The bin file is also included because it has JSON files which include positions of sprites in the photo atlas.

## Overview
The game consits of a player searching for miscrits on different objects such as trees, rocks,... It uses the pixel perfect collision detection. It also has very simple character animations using different images of the character which are the shown in a fluid succession to make it look like a fluid motion. When the user finds a miscrit, its then moved to the Battle state where the user can use the already gathered miscrits to beat the newly found one. I make great use of the texture atlas here, so that I don't need to load multiple textures, which would make the game unoptimizied.
