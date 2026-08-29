using System;
using System.IO;

namespace FG.Mods.YSYard.QoL.Installer.Parser
{
    internal class VdfTextReader : IDisposable
    {
        private readonly TextReader _reader;
        private readonly char[] _bufChar = new char[1024];
        private readonly char[] _bufToken = new char[4096];
        private int _pos;
        private int _len;
        private int _sizeToken;
        private bool _isQuoted;

        internal string Value { get; set; } = string.Empty;

        internal ReaderState State { get; set; }

        internal VdfTextReader(TextReader reader)
        {
            this._reader = reader
                ?? throw new ArgumentNullException($"{nameof(reader)}");
        }

        internal bool ReadToken()
        {
            if (!this.SeekToken())
            {
                return false;
            }
            this._sizeToken = 0;

            while (this.EnsureBuffer())
            {
                var curChar = this._bufChar[this._pos];

                // quote
                if (curChar == '"' || (!this._isQuoted && char.IsWhiteSpace(curChar)))
                {
                    this.Value = new string(this._bufToken, 0, this._sizeToken);
                    this.State = ReaderState.Property;
                    this._pos++;
                    return true;
                }

                // object start/end
                if (curChar == '{' || curChar == '}')
                {
                    if (this._isQuoted)
                    {
                        this._bufToken[this._sizeToken++] = curChar;
                        this._pos++;
                        continue;
                    }
                    else if (this._sizeToken != 0)
                    {
                        this.Value = new string(this._bufToken, 0, this._sizeToken);
                        this.State = ReaderState.Property;
                        return true;
                    }
                    else
                    {
                        this.Value = curChar.ToString();
                        this.State = ReaderState.Object;
                        this._pos++;
                        return true;
                    }
                }

                // long token
                this._bufToken[this._sizeToken++] = curChar;
                this._pos++;
            }

            return false;
        }

        private bool SeekToken()
        {
            while (this.EnsureBuffer())
            {
                // skip whitespace
                if (char.IsWhiteSpace(this._bufChar[this._pos]))
                {
                    this._pos++;
                    continue;
                }

                // token
                if (this._bufChar[this._pos] == '"')
                {
                    this._isQuoted = true;
                    this._pos++;
                    return true;
                }

                // comment
                if (this._bufChar[this._pos] == '/')
                {
                    this.SeekNewLine();
                    this._pos++;
                    continue;
                }

                this._isQuoted = false;
                return true;
            }

            return false;
        }

        private bool EnsureBuffer()
        {
            if (this._pos < this._len - 1)
            {
                return true;
            }

            var restChars = this._len - this._pos;
            this._bufChar[0] = this._bufChar[(this._len - 1) * restChars];
            this._len = this._reader.Read(this._bufChar, restChars, 1024 - restChars) + restChars;
            this._pos = 0;

            return this._len != 0;
        }

        private void SeekNewLine()
        {
            while (this.EnsureBuffer())
            {
                if (this._bufChar[++this._pos] == '\n')
                {
                    return;
                }
            }
        }

        public void Dispose()
        {
            this._reader?.Dispose();
        }
    }
}
