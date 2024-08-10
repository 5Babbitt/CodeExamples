# CodeExamples
 This is a simple collection of Unity-C# code examples from my two most recent projects that are still in active development.  
 All the Code included is around 95%-99% done by me as is the nature of projects with multiple people that they would eventually make small changes to my own scripts.  
 It's always difficult posting samples of code as we improve all the time and that is why I elected to show some scripts from my active projects as I believe that shows my most up to date skills in programming.  

## Aetherline Defense
These samples come from a game I am working on, in my free time, in a team of 6 and hoping to release on steam eventually.  
A cooperative action defense game where you and 3 friends transport and defend passengers and cargo across a wasteland swarming with eldritch monsters on their combat equipped locomotive.  
  
My Responsibilities Included:
- Player Movement, Interaction, Health, Camera and Animation systems
  - Combat is still in development as of writing this
- Game Input   
  - Making use of an abstraction layer using a scriptable object to essentially broadcast the input events to any script that needs it
- Multiplayer Networking, using Unity's Netcode for Gameobjects
- Lobby Systems
  - Initially built with Unity's lobby and relay service, I switched to Steamworks for our plan to release on Steam
- Steam Integration
- Game and Player Management
  - Handle game state and player spawning

Below is a sample video from early devlopment showcasing the camera zones.  

https://github.com/user-attachments/assets/d4770c6e-883c-45cf-8531-0a3c73e0dc1d

## Shadow of the Alchemist  
This is a submission for Pirate Software Game Jam 15, it was not complete on submission but we have been working to polish it up for our portfolio's  
The jam's theme was Shadow and Alchemy and so we created a game about a boy, experimented on by an old alchemist, that can turn into a shadow, must try to escape the Alchemists tower and gain freedom.
  
My Responsibilities Included:
- Player Movement and interaction
  - Human movment makes use of Unity's Character Controller to handle collision detection.
  - Shadow movement makes use of a custom arc cast used to detect the ground no matter the angle so that the shadow can move along any surface.
    - The code for the arc cast is included in the PhysicsUtils script on my [Custom Tools](https://github.com/5Babbitt/Babbitts-Custom-Tools)
- Interactables System
  - Doors
  - Pickups
  - Pushables
  - Switches
  - etc...


Below is footage of the Shadow's movement with Gizmos enabled for debugging.  


https://github.com/user-attachments/assets/8557ca41-173a-4265-9714-4f1b09478938

