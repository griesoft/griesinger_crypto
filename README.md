# Griesinger Crypto - Encryption Tool

This is a simple Windows Form application that can encrypt and decrypt files and folders.
The application uses __AES 256 bit__ encryption. It also uses server authentication, so the actual key for encryption is stored on a server. This adds an extra layer of control in case your password gets compromised.


### Installation

At the moment you need to fork this project and just build it yourself. I will publish this soon to my Azure storage, so it will be easier to install the app.


### How to use

#### Account registration
At the moment the application is only working in connection to a back-end. I use Azure AppService for that. I've build a REST Api that communicates and authenticates the user. I will make a new project and push the back-end code to GitHub also, so you can just create your own back-end from that. 
But for now in order to use this application you need to use my server. 

The name of the server is __`griesingercrypto`__.

After entering a username and your password for the first time, hit the __Create new account__ button. 

##### `Your password is never saved as plain text on my server!`


#### File encryption
Make sure you've entered server name, username and password in advance, otherways the button for encryption won't get active.

Now you need to pay attention to what type your resource will be and check the right radio button. Is it a file or a folder. After that hit the __Find Resource__ button and select it from the file explorer dialog.

After that just hit the __Encrypt__ button and your file or folder will be encrypted. The output will be saved to the same directory where the input resource was located.


#### File decryption
Make sure you've entered server name, username and password in advance, otherways the button for decryption won't get active.

`When decrypting, alway choose file as the type of your resource.`

Hit the __Find Resource__ button and look for the file you want to decrypt. The file will (needs to) have the __.jgc__ extension.

After you found your file, hit the __Decrypt__ button and wait for it.


### FAQ

> Can I recover my encrypted files when I forgot my password?

If you registered your account on my server, absolutly not! The reason is that I don't have enough information about a user for account recovery and therefor I can't identify if a key really belongs to you or not. The solution is to just build your own back-end and connect to that. In that way you will always have access to your key as long as you have access to your back-end.

> How save is the encryption?

Look it up on [Wikipedia here](https://en.wikipedia.org/wiki/Advanced_Encryption_Standard). Basicly AES 256 bit encryption doesn't break till this day, as far as I know. The situation will be different with the rising of quantum computers. Till then your files are save. As a side note, even that the encryption itself is very strong, keep in mind that your password should be strong too.

> Why use a server as an extra layer?

Well it increases security by a couple of points.
1. You could always block the access to your key, if you feel that your password has leaked or something. 
2. You have protection against password brute force attacks. The server keeps count of failed tries and can lock your account if the number of failed tries exeed a certain amount.
3. You could change your password at anytime without the need of first decrypting all your files with your old password and then encrypt again with your new pw.
4. The server can even send a self destruct command that destroys the decrypted file, if not catched by the person trying to gain access to your files.

### Disclaimers

Use this tool and my server in combination or seperatly only under your own risk. I can and will not guarantee the uptime and availablity of my server. I also do not take responsibility of anykind of data lose or corruption of data.

This is a fun project of mine and I can not promise anykind of support if you run into problems using this application.

So __only__ use this application at __your own risk__!
