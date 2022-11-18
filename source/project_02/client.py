# Import socket module
import socket
 
 
def Main():
    # local host IP '127.0.0.1'
    host = '127.0.0.1'
 
    # Define the port on which you want to connect
    port = 8080
 
    s = socket.socket(socket.AF_INET,socket.SOCK_STREAM)
 
    # connect to server on local computer
    s.connect((host,port))
 
    # message you send to server
    o_buffer = "shaurya says geeksforgeeks"
    i_buffer = ""
    while True:
        print("==============================\n\n")
        # message received from server
        i_buffer = str(s.recv(1024), encoding='utf8')
        print(i_buffer)
 
        # ask the client whether he wants to continue
        o_buffer = input('\nOpção:')
        s.send(o_buffer.encode('utf8'))

        i_buffer = str(s.recv(1024), encoding='utf8')

        if i_buffer == "1":
            o_buffer = input('\nDigite seu nome:')
            s.send(o_buffer.encode('utf8'))
            i_buffer = ""
        
        elif i_buffer == "0":
            i_buffer = ""
            break
        
        else:
            i_buffer = ""
            print("\n pa pa pa" + i_buffer + "\n")

    
    s.close()
 
if __name__ == '__main__':
    Main()