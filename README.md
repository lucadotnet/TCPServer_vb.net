# TCPServer_vb.net
A simple TCP Server using vb.net

Hi, I've made this project to simple comunicate something (a generic message) from my Raspberry via socket TCP. This project also is used to write my actual power consumation and a totalizer of KW used by my house on a SQL database. All TCP Client has a separated thread.

This message is composed by: 
                          - command: 1 char 
                          - integer: 32 bit -> 4 char 
                          - split char: ";" 
                          - integer: 32 bit -> 4 char 
                          - terminator char: "$"

After messaged received, it is splitted and in base of command, and it made a different action on a database. First integer is a actual value of power consumation, second integer is a totalizer of KW used.

In my case, a message can be like "W1234;4567$" and it's means: 
                          - command: W -> write data to database 
                          - Actual power KW/h: 1234 kw/h 
                          - split char: ";" 
                          - Totalizer of KW: 4567 kw 
                          - terminator char: "$"

When a message received, a function called DoCommand do various action in base of a "command char".

If you have any suggest, or if you do better my code, please upload your version. Thanks
