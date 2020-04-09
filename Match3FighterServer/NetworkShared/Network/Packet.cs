using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkShared.Network
{
    public class Packet : IDisposable
    {
        private List<byte> buffer;
        private byte[] readableBuffer;
        private int readPos;

        /// <summary>
        /// Creates a new empty packet (without an ID)
        /// </summary>
        public Packet()
        {
            buffer = new List<byte>();
            readPos = 0;
        }

        /// <summary>
        /// Creates a new packet with a given ID
        /// </summary>
        /// <param name="type">The packet ID</param>
        public Packet(int type)
        {
            buffer = new List<byte>();
            readPos = 0;

            Write(type);
        }

        /// <summary>
        /// Creates a packet from which data can be read
        /// </summary>
        /// <param name="data">The bytes to add to the packet</param>
        public Packet(byte[] data)
        {
            buffer = new List<byte>();
            readPos = 0;

            SetBytes(data);
        }

        #region Functions

        /// <summary>
        /// Sets the packet's content and prepares it to be read
        /// </summary>
        /// <param name="data">The bytes to add to the packet</param>
        public void SetBytes(byte[] data)
        {
            Write(data);
            readableBuffer = buffer.ToArray();
        }

        /// <summary>
        /// Inserts the length of the packet's content at the start of the buffer
        /// </summary>
        public void WriteLength()
        {
            buffer.InsertRange(0,
                BitConverter.GetBytes(buffer.Count)); // Insert the byte length of the packet at the very beginning
        }

        /// <summary>
        /// Gets the packet's content in array form
        /// </summary>
        public byte[] ToArray()
        {
            readableBuffer = buffer.ToArray();
            return readableBuffer;
        }

        /// <summary>
        /// Gets the length of the packet's content
        /// </summary>
        public int Length()
        {
            return buffer.Count; // Return the length of buffer
        }

        /// <summary>
        /// Gets the length of the unread data contained in the packet
        /// </summary>
        public int UnreadLength()
        {
            return Length() - readPos; // Return the remaining length (unread)
        }

        /// <summary>
        /// Resets the packet instance to allow it to be reused
        /// </summary>
        /// <param name="shouldReset">Whether or not to reset the packet</param>
        public void Reset(bool shouldReset = true)
        {
            if (shouldReset)
            {
                buffer.Clear(); // Clear buffer
                readableBuffer = null;
                readPos = 0; // Reset readPos
            }
            else
            {
                readPos -= 4; // "Unread" the last read int
            }
        }

        #endregion

        #region Write

        /// <summary>Adds an array of bytes to the packet</summary>
        /// <param name="value">The byte array to add</param>
        public void Write(byte[] value)
        {
            buffer.AddRange(value);
        }

        /// <summary>Adds an int to the packet</summary>
        /// <param name="value">The int to add</param>
        public void Write(int value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }

        /// <summary>Adds a string to the packet</summary>
        /// <param name="value">The string to add</param>
        public void Write(string value)
        {
            Write(value.Length);
            buffer.AddRange(Encoding.ASCII.GetBytes(value));
        }

        #endregion

        #region Read

        /// <summary>
        /// Reads all bytes from the packet
        /// </summary>
        /// <returns></returns>
        public byte[] ReadAllBytes()
        {
            if (buffer.Count > readPos)
            {
                byte[] value = buffer.GetRange(readPos, buffer.Count - readPos).ToArray();
                readPos += value.Length;

                return value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Reads an array of bytes from the packet
        /// </summary>
        /// <param name="length">The length of the byte array</param>
        public byte[] ReadBytes(int length)
        {
            if (buffer.Count > readPos)
            {
                byte[] value = buffer.GetRange(readPos, length).ToArray();
                readPos += length;

                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'byte[]'!");
            }
        }

        /// <summary>
        /// Reads an int from the packet
        /// </summary>
        public int ReadInt()
        {
            if (buffer.Count > readPos)
            {
                int value = BitConverter.ToInt32(readableBuffer, readPos);
                readPos += 4;

                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'int'!");
            }
        }

        /// <summary>
        /// Reads a string from the packet
        /// </summary>
        public string ReadString()
        {
            try
            {
                int length = ReadInt();
                string value = Encoding.ASCII.GetString(readableBuffer, readPos, length);
                readPos += length;

                return value;
            }
            catch
            {
                throw new Exception("Could not read value of type 'string'!");
            }
        }

        #endregion

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
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
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}