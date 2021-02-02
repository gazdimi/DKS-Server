using System.Collections;
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
    player_health,
    regenerated_player,
    generate_tile,
    generate_room
}

//sent from client to server
public enum ClientPackets
{
    welcomeReceived = 1,
    player_movement,
    shoot
}

public class Packet : IDisposable                               //interface that provides a mechanism for releasing unmanaged resources
{
    private List<byte> buffer;                                  //packet's content
    private byte[] readableBuffer;                              //ready packet to be read
    private int readPos;

    public Packet()                                             //constructor for creating a new empty packet
    {
        buffer = new List<byte>();
        readPos = 0;                                            //points to a position for reading packet info
    }

    public Packet(int id)                                      //overloaded constructor for creating a new empty packet with the given id (needs for sending data)
    {
        buffer = new List<byte>();
        readPos = 0;

        Write(id);                                             //write packet id to the buffer
    }

    public Packet(byte[] data)                                 //overloaded constructor for creating a packet from which the given data can be read (used for receiving)
    {
        buffer = new List<byte>();
        readPos = 0;

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

    public void InsertInt(int value)                           //insert given integer at the start of the buffer (int is equal to 4 bytes)
    {
        buffer.InsertRange(0, BitConverter.GetBytes(value));
    }

    public byte[] ToArray()                                     //convert packet's content to an array in order to be readable
    {
        readableBuffer = buffer.ToArray();
        return readableBuffer;
    }

    public int Length()                                         //get packet's content length
    {
        return buffer.Count;
    }

    public int UnreadLength()                                   //get length of unread data contained in the packet
    {
        return Length() - readPos;                              //return the remaining length of the unread data
    }

    public void Reset(bool shouldReset = true)                 //clear packet so it can be reused
    {
        if (shouldReset)                                       //reset packet
        {
            buffer.Clear();                                     //clear buffer
            readableBuffer = null;
            readPos = 0;                                        //reset position
        }
        else
        {
            readPos -= 4;                                       //"unread" the last read int (integer equals to 4 bytes)
        }
    }
    //------------------------------------------------------------------------------------------------------------------------------

    public void Write(byte value)                              //add given byte to the packet (buffer)
    {
        buffer.Add(value);
    }

    public void Write(byte[] value)                            //add given array of bytes to the packet
    {
        buffer.AddRange(value);
    }

    public void Write(short value)                             //add given short integer to the packet
    {
        buffer.AddRange(BitConverter.GetBytes(value));
    }

    public void Write(int value)                               //add given int to the packet
    {
        buffer.AddRange(BitConverter.GetBytes(value));
    }

    public void Write(long value)                              //add given long int to the packet
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

    public byte ReadByte(bool moveReadPos = true)              //read a byte from the packet
    {
        if (buffer.Count > readPos)
        {
            //if unread bytes exist
            byte value = readableBuffer[readPos];             //get the byte at the readPos position
            if (moveReadPos)                                  //if true the move buffer's read position
            {
                readPos += 1;
            }
            return value;                                     //return the byte
        }
        else
        {
            throw new Exception("Could not read value of type 'byte'!");
        }
    }

    public byte[] ReadBytes(int length, bool moveReadPos = true)  //reads an array of bytes from the packet (length of the byte array
    {
        if (buffer.Count > readPos)
        {
            //if unread bytes exist
            byte[] value = buffer.GetRange(readPos, length).ToArray();    //get the bytes at the readPos position with a range of given length
            if (moveReadPos)
            {
                readPos += length;                                         //increase readPos by given length
            }
            return value;                                                  //return the bytes
        }
        else
        {
            throw new Exception("Could not read value of type 'byte[]'...");
        }
    }

    public short ReadShort(bool moveReadPos = true)                        //read a short integer from packet
    {
        if (buffer.Count > readPos)
        {
            //if unread bytes exist
            short value = BitConverter.ToInt16(readableBuffer, readPos);   //convert the bytes to a short
            if (moveReadPos)
            {
                readPos += 2;                                               //increase readPos by 2 (short int equals to 16 bits = 2 bytes)
            }
            return value;                                                   //return the short
        }
        else
        {
            throw new Exception("Could not read value of type 'short'...");
        }
    }

    public int ReadInt(bool moveReadPos = true)                            //read an int from the packet
    {
        if (buffer.Count > readPos)
        {
            //if unread bytes exist
            int value = BitConverter.ToInt32(readableBuffer, readPos);      //convert the bytes to an int
            if (moveReadPos)
            {
                readPos += 4;                                               //an int equals to 4 bytes
            }
            return value;                                                   //return the int
        }
        else
        {
            throw new Exception("Could not read value of type 'int'...");
        }
    }

    public long ReadLong(bool moveReadPos = true)                          //read a long from the packet
    {
        if (buffer.Count > readPos)
        {
            //if unread bytes exist
            long value = BitConverter.ToInt64(readableBuffer, readPos);     //convert the bytes to a long
            if (moveReadPos)
            {
                readPos += 8;                                               //increase readPos by 8 (a long int equals to 64 bits)
            }
            return value;                                                   //return the long
        }
        else
        {
            throw new Exception("Could not read value of type 'long'...");
        }
    }

    public float ReadFloat(bool moveReadPos = true)                        //read a float from the packet
    {
        if (buffer.Count > readPos)
        {
            //unread bytes exist
            float value = BitConverter.ToSingle(readableBuffer, readPos);  //convert the bytes to a float
            if (moveReadPos)
            {
                readPos += 4;                                               //increase readPos by 4 (float equals to 32 bits)
            }
            return value;                                                   //return the float
        }
        else
        {
            throw new Exception("Could not read value of type 'float'...");
        }
    }

    public bool ReadBool(bool moveReadPos = true)                          //read a bool from the packet
    {
        if (buffer.Count > readPos)
        {
            //if unread bytes exist
            bool value = BitConverter.ToBoolean(readableBuffer, readPos);   //convert the bytes to a bool
            if (moveReadPos)
            {
                readPos += 1;                                               //increase readPos by 1
            }
            return value;                                                   //return the bool
        }
        else
        {
            throw new Exception("Could not read value of type 'bool'...");
        }
    }

    public string ReadString(bool moveReadPos = true)                      //read a string from the packet
    {
        try
        {
            int length = ReadInt();                                        //get string's length
            string value = Encoding.ASCII.GetString(readableBuffer, readPos, length); //convert the bytes to a string
            if (moveReadPos && value.Length > 0)
            {
                readPos += length;                                         //increase readPos by the length of the string
            }
            return value;                                                  //return the string
        }
        catch
        {
            throw new Exception("Could not read value of type 'string'!");
        }
    }

    public Vector3 ReadVector3(bool moveReadPos = true)                    //read Vector3 from the packet
    {
        return new Vector3(ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos));     //return the created instance
    }

    public Quaternion ReadQuaternion(bool moveReadPos = true)              //read Quaternion from the packet
    {
        return new Quaternion(ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos));
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
                readPos = 0;
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
