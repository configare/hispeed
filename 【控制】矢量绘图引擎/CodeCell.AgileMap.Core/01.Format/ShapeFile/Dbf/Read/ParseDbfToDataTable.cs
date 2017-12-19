using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;

namespace CodeCell.AgileMap.Core
{
    // Read an entire standard DBF file into a DataTable
    public static class ParseDbfToDataTable
    {
        // This is the file header for a DBF. We do this special layout with everything
        // packed so we can read straight from disk into the structure to populate it
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        private struct DBFHeader
        {
            public byte version;
            public byte updateYear;
            public byte updateMonth;
            public byte updateDay;
            public Int32 numRecords;
            public Int16 headerLen;
            public Int16 recordLen;
            public Int16 reserved1;
            public byte incompleteTrans;
            public byte encryptionFlag;
            public Int32 reserved2;
            public Int64 reserved3;
            public byte MDX;
            public byte language;
            public Int16 reserved4;
        }

        // This is the field descriptor structure. There will be one of these for each column in the table.
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        private struct FieldDescriptor
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
            public string fieldName;
            public char fieldType;
            public Int32 address;
            public byte fieldLen;
            public byte count;
            public Int16 reserved1;
            public byte workArea;
            public Int16 reserved2;
            public byte flag;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            public byte[] reserved3;
            public byte indexFlag;
        }

        public static string[] GetFields(string dbfFile)
        {
           // If there isn't even a file, just return an empty DataTable
            if ((false == File.Exists(dbfFile)))
                return null;
            BinaryReader br = null;
            try
            {
                // Read the header into a buffer
                br = new BinaryReader(File.OpenRead(dbfFile));
                br.BaseStream.Seek(29, SeekOrigin.Begin); //Seek to encoding flag
                Encoding _fileEncoding = GetDbaseLanguageDriver(br.ReadByte()); //Read and parse Language driver
                br.BaseStream.Seek(0, SeekOrigin.Begin);
                byte[] buffer = br.ReadBytes(Marshal.SizeOf(typeof(DBFHeader)));
                // Marshall the header into a DBFHeader structure
                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                DBFHeader header = (DBFHeader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(DBFHeader));
                handle.Free();
                // Read in all the field descriptors. Per the spec, 13 (0D) marks the end of the field descriptors
                ArrayList fields = new ArrayList();
                List<string> fieldnames = new List<string>(); 
                int NumberOfColumns = (header.headerLen - 31) / 32;
                for (int i = 0; i < NumberOfColumns; i++)
                {
                    buffer = br.ReadBytes(Marshal.SizeOf(typeof(FieldDescriptor)));
                    handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                    fields.Add((FieldDescriptor)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(FieldDescriptor)));
                    handle.Free();
                    fieldnames.Add(((FieldDescriptor)fields[i]).fieldName);
                }
                return fieldnames.ToArray();
            }
            finally 
            {
                if (br != null)
                    br.Close();
            }
        }

        // Read an entire standard DBF file into a DataTable
        // Read an entire standard DBF file into a DataTable
        public static DataTable ReadDBF(string dbfFile)
        {
            DataTable dt = new DataTable();
            BinaryReader recReader;
            string number;
            string year;
            string month;
            string day;
            DataRow row;

            // If there isn't even a file, just return an empty DataTable
            if ((false == File.Exists(dbfFile)))
            {
                return dt;
            }

            BinaryReader br = null;
            try
            {
                // Read the header into a buffer
                br = new BinaryReader(File.OpenRead(dbfFile));
                br.BaseStream.Seek(29, SeekOrigin.Begin); //Seek to encoding flag
                Encoding _fileEncoding = GetDbaseLanguageDriver(br.ReadByte()); //Read and parse Language driver
                br.BaseStream.Seek(0, SeekOrigin.Begin);
                byte[] buffer = br.ReadBytes(Marshal.SizeOf(typeof(DBFHeader)));
                // Marshall the header into a DBFHeader structure
                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                DBFHeader header = (DBFHeader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(DBFHeader));
                handle.Free();
                // Read in all the field descriptors. Per the spec, 13 (0D) marks the end of the field descriptors
                ArrayList fields = new ArrayList();
                int NumberOfColumns = (header.headerLen - 31) / 32;
                for (int i = 0; i < NumberOfColumns; i++)
                {
                    buffer = br.ReadBytes(Marshal.SizeOf(typeof(FieldDescriptor)));
                    handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                    fields.Add((FieldDescriptor)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(FieldDescriptor)));
                    handle.Free();
                }
                // Read in the first row of records, we need this to help determine column types below
                ((FileStream)br.BaseStream).Seek(header.headerLen + 1, SeekOrigin.Begin);
                buffer = br.ReadBytes(header.recordLen);
                recReader = new BinaryReader(new MemoryStream(buffer));
                // Create the columns in our new DataTable
                DataColumn col = null;
                foreach (FieldDescriptor field in fields)
                {
                    number = Encoding.ASCII.GetString(recReader.ReadBytes(field.fieldLen));
                    switch (field.fieldType)
                    {
                        case 'N':
                            if (number.IndexOf(".") > -1)
                            {
                                col = new DataColumn(field.fieldName, typeof(decimal));
                            }
                            else
                            {
                                col = new DataColumn(field.fieldName, typeof(int));
                            }
                            break;
                        case 'C':
                            col = new DataColumn(field.fieldName, typeof(string));
                            break;
                        case 'D':
                            col = new DataColumn(field.fieldName, typeof(DateTime));
                            break;
                        case 'L':
                            col = new DataColumn(field.fieldName, typeof(bool));
                            break;
                        case 'F':
                            col = new DataColumn(field.fieldName, typeof(Double));
                            break;
                    }
                    dt.Columns.Add(col);
                }

                // Skip past the end of the header. 
                ((FileStream)br.BaseStream).Seek(header.headerLen, SeekOrigin.Begin);

                // Read in all the records
                for (int counter = 0; counter <= header.numRecords - 1; counter++)
                {
                    // First we'll read the entire record into a buffer and then read each field from the buffer
                    // This helps account for any extra space at the end of each record and probably performs better
                    buffer = br.ReadBytes(header.recordLen);
                    recReader = new BinaryReader(new MemoryStream(buffer));

                    // All dbf field records begin with a deleted flag field. Deleted - 0x2A (asterisk) else 0x20 (space)
                    if (recReader.ReadChar() == '*')
                    {
                        continue;
                    }

                    // Loop through each field in a record
                    row = dt.NewRow();
                    foreach (FieldDescriptor field in fields)
                    {
                        switch (field.fieldType)
                        {
                            case 'N':  // Number
                                // If you port this to .NET 2.0, use the Decimal.TryParse method
                                number = Encoding.ASCII.GetString(recReader.ReadBytes(field.fieldLen));
                                if (IsNumber(number))
                                {
                                    if (number.IndexOf(".") > -1)
                                    {
                                        row[field.fieldName] = decimal.Parse(number);
                                    }
                                    else
                                    {
                                        row[field.fieldName] = int.Parse(number);
                                    }
                                }
                                else
                                {
                                    row[field.fieldName] = 0;
                                }

                                break;

                            case 'C': // String
                                string sv = _fileEncoding.GetString(recReader.ReadBytes(field.fieldLen)).Trim().Replace("'", "‘");
                                string actualValue = string.Empty;
                                char[] valueArray = sv.ToCharArray();
                                foreach (char c in valueArray)
                                {
                                    if (c == '\0')
                                        break;
                                    actualValue += c;
                                }
                                row[field.fieldName] = actualValue;
                                break;

                            case 'D': // Date (YYYYMMDD)
                                year = Encoding.ASCII.GetString(recReader.ReadBytes(4));
                                month = Encoding.ASCII.GetString(recReader.ReadBytes(2));
                                day = Encoding.ASCII.GetString(recReader.ReadBytes(2));
                                row[field.fieldName] = System.DBNull.Value;
                                try
                                {
                                    if (IsNumber(year) && IsNumber(month) && IsNumber(day))
                                    {
                                        if (year.Trim() == string.Empty)
                                            year = "2000";
                                        if (month.Trim() == string.Empty)
                                            month = "01";
                                        if (day.Trim() == string.Empty)
                                            day = "01";
                                        if ((Int32.Parse(year) > 1900))
                                        {
                                            row[field.fieldName] = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(day));
                                        }
                                    }
                                }
                                catch
                                { }

                                break;

                            case 'L': // Boolean (Y/N)
                                if ('Y' == recReader.ReadByte())
                                {
                                    row[field.fieldName] = true;
                                }
                                else
                                {
                                    row[field.fieldName] = false;
                                }

                                break;

                            case 'F':
                                number = Encoding.ASCII.GetString(recReader.ReadBytes(field.fieldLen));
                                float dv = 0;
                                if (float.TryParse(number, out dv))
                                {
                                    row[field.fieldName] = double.Parse(number);
                                }
                                //if (IsNumber(number))
                                //{
                                //    row[field.fieldName] = double.Parse(number);
                                //}
                                break;
                        }
                    }

                    recReader.Close();
                    dt.Rows.Add(row);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (null != br)
                {
                    br.Close();
                }
            }

            return dt;
        }

        /// <summary>
        /// Simple function to test is a string can be parsed. There may be a better way, but this works
        /// If you port this to .NET 2.0, use the new TryParse methods instead of this
        /// </summary>
        /// <param name="number">string to test for parsing</param>
        /// <returns>true if string can be parsed</returns>
        public static bool IsNumber(string numberString)
        {
            char[] numbers = numberString.ToCharArray();

            foreach (char number in numbers)
            {
                if ((number < 48 || number > 57) && number != 46 && number != 32)
                {
                    return false;
                }
            }

            return true;
        }

        private static Encoding GetDbaseLanguageDriver(byte dbasecode)
        {
            switch (dbasecode)
            {
                case 0x01: return System.Text.Encoding.GetEncoding(437); //DOS USA code page 437 
                case 0x02: return System.Text.Encoding.GetEncoding(850); // DOS Multilingual code page 850 
                case 0x03: return System.Text.Encoding.GetEncoding(1252); // Windows ANSI code page 1252 
                case 0x04: return System.Text.Encoding.GetEncoding(10000); // Standard Macintosh 
                case 0x08: return System.Text.Encoding.GetEncoding(865); // Danish OEM
                case 0x09: return System.Text.Encoding.GetEncoding(437); // Dutch OEM
                case 0x0A: return System.Text.Encoding.GetEncoding(850); // Dutch OEM Secondary codepage
                case 0x0B: return System.Text.Encoding.GetEncoding(437); // Finnish OEM
                case 0x0D: return System.Text.Encoding.GetEncoding(437); // French OEM
                case 0x0E: return System.Text.Encoding.GetEncoding(850); // French OEM Secondary codepage
                case 0x0F: return System.Text.Encoding.GetEncoding(437); // German OEM
                case 0x10: return System.Text.Encoding.GetEncoding(850); // German OEM Secondary codepage
                case 0x11: return System.Text.Encoding.GetEncoding(437); // Italian OEM
                case 0x12: return System.Text.Encoding.GetEncoding(850); // Italian OEM Secondary codepage
                case 0x13: return System.Text.Encoding.GetEncoding(932); // Japanese Shift-JIS
                case 0x14: return System.Text.Encoding.GetEncoding(850); // Spanish OEM secondary codepage
                case 0x15: return System.Text.Encoding.GetEncoding(437); // Swedish OEM
                case 0x16: return System.Text.Encoding.GetEncoding(850); // Swedish OEM secondary codepage
                case 0x17: return System.Text.Encoding.GetEncoding(865); // Norwegian OEM
                case 0x18: return System.Text.Encoding.GetEncoding(437); // Spanish OEM
                case 0x19: return System.Text.Encoding.GetEncoding(437); // English OEM (Britain)
                case 0x1A: return System.Text.Encoding.GetEncoding(850); // English OEM (Britain) secondary codepage
                case 0x1B: return System.Text.Encoding.GetEncoding(437); // English OEM (U.S.)
                case 0x1C: return System.Text.Encoding.GetEncoding(863); // French OEM (Canada)
                case 0x1D: return System.Text.Encoding.GetEncoding(850); // French OEM secondary codepage
                case 0x1F: return System.Text.Encoding.GetEncoding(852); // Czech OEM
                case 0x22: return System.Text.Encoding.GetEncoding(852); // Hungarian OEM
                case 0x23: return System.Text.Encoding.GetEncoding(852); // Polish OEM
                case 0x24: return System.Text.Encoding.GetEncoding(860); // Portuguese OEM
                case 0x25: return System.Text.Encoding.GetEncoding(850); // Portuguese OEM secondary codepage
                case 0x26: return System.Text.Encoding.GetEncoding(866); // Russian OEM
                case 0x37: return System.Text.Encoding.GetEncoding(850); // English OEM (U.S.) secondary codepage
                case 0x40: return System.Text.Encoding.GetEncoding(852); // Romanian OEM
                case 0x4D: return System.Text.Encoding.GetEncoding(936); // Chinese GBK (PRC)
                case 0x4E: return System.Text.Encoding.GetEncoding(949); // Korean (ANSI/OEM)
                case 0x4F: return System.Text.Encoding.GetEncoding(950); // Chinese Big5 (Taiwan)
                case 0x50: return System.Text.Encoding.GetEncoding(874); // Thai (ANSI/OEM)
                case 0x57: return System.Text.Encoding.GetEncoding(1252); // ANSI
                case 0x58: return System.Text.Encoding.GetEncoding(1252); // Western European ANSI
                case 0x59: return System.Text.Encoding.GetEncoding(1252); // Spanish ANSI
                case 0x64: return System.Text.Encoding.GetEncoding(852); // Eastern European MS朌OS
                case 0x65: return System.Text.Encoding.GetEncoding(866); // Russian MS朌OS
                case 0x66: return System.Text.Encoding.GetEncoding(865); // Nordic MS朌OS
                case 0x67: return System.Text.Encoding.GetEncoding(861); // Icelandic MS朌OS
                case 0x68: return System.Text.Encoding.GetEncoding(895); // Kamenicky (Czech) MS-DOS 
                case 0x69: return System.Text.Encoding.GetEncoding(620); // Mazovia (Polish) MS-DOS 
                case 0x6A: return System.Text.Encoding.GetEncoding(737); // Greek MS朌OS (437G)
                case 0x6B: return System.Text.Encoding.GetEncoding(857); // Turkish MS朌OS
                case 0x6C: return System.Text.Encoding.GetEncoding(863); // French朇anadian MS朌OS
                case 0x78: return System.Text.Encoding.GetEncoding(950); // Taiwan Big 5
                case 0x79: return System.Text.Encoding.GetEncoding(949); // Hangul (Wansung)
                case 0x7A: return System.Text.Encoding.GetEncoding(936); // PRC GBK
                case 0x7B: return System.Text.Encoding.GetEncoding(932); // Japanese Shift-JIS
                case 0x7C: return System.Text.Encoding.GetEncoding(874); // Thai Windows/MS朌OS
                case 0x7D: return System.Text.Encoding.GetEncoding(1255); // Hebrew Windows 
                case 0x7E: return System.Text.Encoding.GetEncoding(1256); // Arabic Windows 
                case 0x86: return System.Text.Encoding.GetEncoding(737); // Greek OEM
                case 0x87: return System.Text.Encoding.GetEncoding(852); // Slovenian OEM
                case 0x88: return System.Text.Encoding.GetEncoding(857); // Turkish OEM
                case 0x96: return System.Text.Encoding.GetEncoding(10007); // Russian Macintosh 
                case 0x97: return System.Text.Encoding.GetEncoding(10029); // Eastern European Macintosh 
                case 0x98: return System.Text.Encoding.GetEncoding(10006); // Greek Macintosh 
                case 0xC8: return System.Text.Encoding.GetEncoding(1250); // Eastern European Windows
                case 0xC9: return System.Text.Encoding.GetEncoding(1251); // Russian Windows
                case 0xCA: return System.Text.Encoding.GetEncoding(1254); // Turkish Windows
                case 0xCB: return System.Text.Encoding.GetEncoding(1253); // Greek Windows
                case 0xCC: return System.Text.Encoding.GetEncoding(1257); // Baltic Windows
                default:
                    return System.Text.Encoding.GetEncoding(936); 
            }

        }
    }
}