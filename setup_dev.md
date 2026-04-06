# This tutorial is for Jetbrains Rider

## Update Rider to the latest version

## Creating a dev container

Open **Settings -> File -> Remote Development**

<img width="347" height="544" alt="image" src="https://github.com/user-attachments/assets/c7f741c2-236f-4809-94b4-2d1a80dbe55b" />

On the left click devcontainers and create new if it doesn t already exist.

<img width="998" height="272" alt="image" src="https://github.com/user-attachments/assets/3657ccd4-9db0-439e-9a44-3f9538ec4df2" />

If it exists just press on the container you want to work on:

<img width="719" height="241" alt="image" src="https://github.com/user-attachments/assets/4032ae18-3744-4d47-9786-d6eb58d78c17" />

If the container does not exist create one.
Select **FROM VCS PROJECT** and put in the following link:

https://github.com/VIA-Plantify/Plantify-BusinessTier.git

<img width="1008" height="312" alt="image" src="https://github.com/user-attachments/assets/9d940f6e-e5e9-4a0a-ac69-b96b0c9ebc10" />

<img width="1008" height="284" alt="image" src="https://github.com/user-attachments/assets/6882b5b8-ed43-441c-ad78-ee717173646c" />

select **development or the branch currently under development** and continue, the ide should setup a remote container

<img width="988" height="196" alt="image" src="https://github.com/user-attachments/assets/11806b2e-2bd2-4afd-a2aa-92a5374bbffb" />


### IMPORTANT !!!
<h3>The  gRPC container from https://github.com/VIA-Plantify/Plantify-DataTier.git must be running before the webAPI</h3>

## Refreshing WebApi Container
inside the devcontainer run: ``scripts/refresh-webapi.sh``

outside container it gets a bit more complicated,
I know only for linux but it should work in WSL:

run: ``chmod +x scripts/refresh-webapi.sh`` to make it executable

afterwards run: ``./scripts/refresh-webapi.sh``

## Making an IDE button for refresh script in Rider

Go to the top of the options where usually the program is being ran

<img width="406" height="216" alt="image" src="https://github.com/user-attachments/assets/03598e4e-6e03-444f-a7d1-ca2e43a01426" />

Click on the arrow nad go to edit configurations

<img width="368" height="196" alt="image" src="https://github.com/user-attachments/assets/20378cab-cbc9-45c0-88fd-976c2d45ea23" />

There you should see a "+" sign

<img width="421" height="196" alt="image" src="https://github.com/user-attachments/assets/39819896-ce8e-448e-8de1-7c9ce32e8867" />

Click on the "+" scroll down and find the shell script option

<img width="368" height="522" alt="image" src="https://github.com/user-attachments/assets/70c645a7-066b-43a9-924a-d480cd364bd1" />

Add the script and the correct path from scripts and apply

<img width="1197" height="617" alt="image" src="https://github.com/user-attachments/assets/b03974f7-5538-45e1-b0fa-c5d6272ad716" />

Now you should have a button that runs the script

<img width="317" height="46" alt="image" src="https://github.com/user-attachments/assets/9cfed1a3-7c74-4098-af56-41f1426b56a5" />
