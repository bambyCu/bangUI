# UI
probably not very nice 

# Hub 
used for asynchronous communication between clients and server/clients

# Wrapper class
this class is used for operations that require comunication between players or their input  
most importantly this class takes care of attacks with attack query which holds all attacks.
it is filled with threads that wait for certain amount of time if attacked player is inactive  
and does not miss that attack he will take damage. Or he can block incoming attack by answering   
prompt in UI

# Game class
this class is most basic implementation of game, everything that can be written without other parts of application, is written in this class  
that means that Game class contains all operations that do not need confirmation from user or do not need to suport comunication between players   

# Low level classes
these are classes that are used in Game class or classes under those classes
- class Player:  
    - Variables:
        Name is name of user in higher levels  
        SeeingDistance is extra range that payer has (Mirino)
        SeeingAttackDistance is extra distance for attacking (Guns)
- class Card:  
    represents playable card in game bang  
    - Variables:  
        Type is used when finding photo of said card, and can be used(is) used in logic in higher classes  
        Id is set by higher class that uses this class, it was added for easier search for specific card  

