using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

//sent from server to client
public enum ServerPackets
{
    welcome = 1,
    generated_player,
    player_position,
    player_rotation,
    disconnected_player,
    generate_Tile,
    generate_IRoom,
    askPen,
    remoteDoor,
    weaponLocation,
    remotePlayerWeapon,
    spawnEnemy,
    spawnMod,
    moveEnemy,
    inCombat,
    returnEnemiesInCombat,
    load_scene
}

//sent from client to server
public enum ClientPackets
{
    welcomeReceived = 1,
    player_movement,
    startgame,
    pen_values,
    hold_weapon,
    askEnemiesForCombat
}

public class Packet : IDisposable                               //interface that provides a mechanism for releasing unmanaged resources
{
    private List<byte> buffer;                                  //packet's content
    private byte[] readableBuffer;                              //ready packet to be read
    private int positionToRead;

    public Packet()                                             //constructor for creating a new empty packet
    {
        buffer = new List<byte>();
        positionToRead = 0;                                     //points to a position for reading packet info
    }

    public Packet(int id)                                      //overloaded constructor for creating a new empty packet with the given id (needs for sending data)
    {
        buffer = new List<byte>();
        positionToRead = 0;

        Write(id);                                             //write packet id to the buffer
    }

    public Packet(byte[] data)                                 //overloaded constructor for creating a packet from which the given data can be read (used for receiving)
    {
        buffer = new List<byte>();
        positionToRead = 0;

        SetBytes(data);                                        //write data (bytes to be added) to the buffer (meaning packet)
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public void SetBytes(byte[] data)                          //sets the packet's content with the given data and prepares it to be read
    {
        Write(data);
        readableBuffer = buffer.ToArray();
    }

    public void WriteLength()                                   //insert packet's content length (byte length) at the start of the buffer
    {
        buffer.InsertRange(0, BitConverter.GetBytes(buffer.Count));
    }

    public byte[] ToArray()                                     //convert packet's content to an array in order to be readable
    {
        readableBuffer = buffer.ToArray();
        return readableBuffer;
    }

    public int ContentLength()                                 //get packet's content length
    {
        return buffer.Count;
    }

    public int UnreadLength()                                  //get length of unread data contained in the packet
    {
        return ContentLength() - positionToRead;               //return the remaining length of the unread data
    }

    public void Clear(bool shouldReset = true)                 //clear packet so it can be reused
    {
        if (shouldReset)                                       //reset packet
        {
            buffer.Clear();                                     //clear buffer
            readableBuffer = null;
            positionToRead = 0;                                 //reset position
        }
        else
        {
            positionToRead -= 4;                                //"unread" the last read int (integer equals to 4 bytes)
        }
    }
    //------------------------------------------------------------------------------------------------------------------------------
  
    public void Write(byte[] value)                            //add given array of bytes to the packet
    {
        buffer.AddRange(value);
    }

    public void Write(int value)                               //add given int to the packet
    {
        buffer.AddRange(BitConverter.GetBytes(value));
    }

    public void Write(float value)                             //add given float to the packet
    {
        buffer.AddRange(BitConverter.GetBytes(value));
    }

    public void Write(bool value)                              //add given boolean to the packet
    {
        buffer.AddRange(BitConverter.GetBytes(value));
    }

    public void Write(string value)                            //add given string to the packet
    {
        Write(value.Length);                                   //add the length of the string to the packet
        buffer.AddRange(Encoding.ASCII.GetBytes(value));       //add the string itself
    }

    public void Write(Vector3 value)                           //add given Vector3 to the packet (position)
    {
        Write(value.x);
        Write(value.y);
        Write(value.z);
    }

    public void Write(Quaternion value)                       //add given Quaternion to the packet (rotation)
    {
        Write(value.x);
        Write(value.y);
        Write(value.z);
        Write(value.w);
    }

    //--------------------------------------------------------------------------------------------------------------------------

    public byte[] ReadBytes(int length, bool movePositionToRead = true)  //reads an array of bytes from the packet (length of the byte array
    {
        if (buffer.Count > positionToRead)
        {
            //if unread bytes exist
            byte[] value = buffer.GetRange(positionToRead, length).ToArray();    //get the bytes at the positionToRead position with a range of given length
            if (movePositionToRead)
            {
                positionToRead += length;                                         //increase positionToRead by given length
            }
            return value;                                                  //return the bytes
        }
        else
        {
            throw new Exception("Could not read value of type 'byte[]'...");
        }
    }

    public int ReadInt(bool movePositionToRead = true)                            //read an int from the packet
    {
        if (buffer.Count > positionToRead)
        {
            //if unread bytes exist
            int value = BitConverter.ToInt32(readableBuffer, positionToRead);      //convert the bytes to an int
            if (movePositionToRead)
            {
                positionToRead += 4;                                               //an int equals to 4 bytes
            }
            return value;                                                   //return the int
        }
        else
        {
            throw new Exception("Could not read value of type 'int'...");
        }
    }

    public float ReadFloat(bool movePositionToRead = true)                        //read a float from the packet
    {
        if (buffer.Count > positionToRead)
        {
            //unread bytes exist
            float value = BitConverter.ToSingle(readableBuffer, positionToRead);  //convert the bytes to a float
            if (movePositionToRead)
            {
                positionToRead += 4;                                               //increase positionToRead by 4 (float equals to 32 bits)
            }
            return value;                                                   //return the float
        }
        else
        {
            throw new Exception("Could not read value of type 'float'...");
        }
    }

    public string ReadString(bool movePositionToRead = true)                      //read a string from the packet
    {
        try
        {
            int length = ReadInt();                                        //get string's length
            string value = Encoding.ASCII.GetString(readableBuffer, positionToRead, length); //convert the bytes to a string
            if (movePositionToRead && value.Length > 0)
            {
                positionToRead += length;                                         //increase positionToRead by the length of the string
            }
            return value;                                                  //return the string
        }
        catch
        {
            throw new Exception("Could not read value of type 'string'!");
        }
    }

    public Vector3 ReadVector3(bool movePositionToRead = true)                    //read Vector3 from the packet
    {
        return new Vector3(ReadFloat(movePositionToRead), ReadFloat(movePositionToRead), ReadFloat(movePositionToRead));     //return the created instance
    }

    //------------------------------------------------------------------------------------------------------------------------------

    private bool disposed = false;                                          //check if Dispose has been called

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                buffer = null;
                readableBuffer = null;
                positionToRead = 0;
            }

            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);                                                  //clean up object
        GC.SuppressFinalize(this);                                      //take this object off the finalization queue and prevent from executing a second time.
    }
}
