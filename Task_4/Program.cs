using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Task_4
{
    public sealed class InfoAboutStudents : IEquatable<InfoAboutStudents>, IComparable, IComparable<InfoAboutStudents>
    {
        private readonly string _surname;
        private readonly string _name;
        private readonly string _patronymic;
        private readonly string _group;
        public string Filename { get; private set; }
        
        private string BuildFileName(int index)
        {
            return $"{_group}_{_surname} {_name} {_patronymic}{index}.pdf";
        }
        public InfoAboutStudents(int group, string information)
        {
            var info = information.Split(' ');
            _surname = info[0];
            _name = info[1];
            _patronymic = info[2];
            _group = group.ToString();
        }

        public override string ToString()
        {
            return $"{_group} {_surname} {_name} {_patronymic}";
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is InfoAboutStudents)
            {
                return Equals(obj as InfoAboutStudents);
            }

            return false;
        }

        public bool Equals(InfoAboutStudents other)
        {
            if (other is null)
            {
                return false;
            }
            return (_group == other._group) && (_name == other._name) &&
                   (_surname == other._surname) && (_patronymic == other._patronymic);
        }

        public int CompareTo(object obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (obj is InfoAboutStudents other)
            {
                return CompareTo(other);
            }
            throw new ArgumentException("Wrong type of ", nameof(obj));
        }

        public int CompareTo(InfoAboutStudents other)
        {
            if (other == null) 
                throw new ArgumentException();
            var tmpString = new StringBuilder();
            tmpString.Append(_group).Append(_surname).Append(_name).Append(_patronymic);
            var thisName = tmpString.ToString();
            tmpString.Clear();
            tmpString.Append(other._group).Append(other._surname).Append(other._name).Append(other._patronymic);

            return string.Compare(thisName, tmpString.ToString(), StringComparison.Ordinal);
        }

        public (string, string) ChangedNamesToInitials(InfoAboutStudents other)
        {
            var firstStudent = new StringBuilder();
            var secondStudent = new StringBuilder();
            
            firstStudent.Append(_group).Append('_').Append(_surname).Append(' ');
            secondStudent.Append(other._group).Append('_').Append(other._surname).Append(' ');
            
            if ((_group == other._group && _surname != other._surname) || (_group != other._group && _surname == other._surname) )
            { 
                firstStudent.Append(_name[0]).Append(". ").Append(_patronymic[0]).Append("..pdf");;
                secondStudent.Append(other._name[0]).Append(". ").Append(other._patronymic[0]).Append("..pdf");
                return (firstStudent.ToString(), secondStudent.ToString());
            }
            
            int minLength = _name.Length < other._name.Length ? _name.Length : other._name.Length;
            for (int i = 0; i < minLength; i++)
            {
                if (_group == other._group && _surname == other._surname &&_name[i] == other._name[i])
                {
                    firstStudent.Append(_name[i]);
                    secondStudent.Append(other._name[i]);
                }
                else
                {
                    firstStudent.Append(_name[i]).Append(". ").Append(_patronymic[0]).Append("..pdf");
                    secondStudent.Append(other._name[i]).Append(". ").Append(other._patronymic[0]).Append("..pdf");
                    return (firstStudent.ToString(), secondStudent.ToString());
                }
            }

            minLength = _patronymic.Length < other._patronymic.Length ? _patronymic.Length : other._patronymic.Length;
            for (int i = 0; i < minLength; i++)
            {
                if (_patronymic[i] == other._patronymic[i])
                {
                    firstStudent.Append(_patronymic[i]);
                    secondStudent.Append(other._patronymic[i]);
                }
                else
                {
                    firstStudent.Append(_patronymic[i]).Append("..pdf");
                    secondStudent.Append(other._patronymic[i]).Append("..pdf");
                    return (firstStudent.ToString(), secondStudent.ToString());
                }
            }
            return (firstStudent.ToString(), secondStudent.ToString());
        }

        public static void CheckingStudentsForRepetition(ref List<InfoAboutStudents> groupList)
        {
            var newFilenames = new string[groupList.Count];
            var index = 2;
            for (var i = 0; i < groupList.Count; i++)
            {
                for (var j = i + 1; j < groupList.Count; j++)
                {
                    if (groupList[i].Filename == groupList[j].Filename)
                    {
                        if (groupList[i].Equals(groupList[j]))
                        {
                            newFilenames[i] = groupList[i].BuildFileName(1);
                            newFilenames[j] = groupList[j].BuildFileName(index++);
                            if (String.Compare(newFilenames[j], groupList[j].Filename, StringComparison.Ordinal) > 0)
                                groupList[j].Filename = newFilenames[j];
                        }
                        else
                        {
                            index = 2;
                            var newFilename = groupList[i].ChangedNamesToInitials(groupList[j]);
                            if (String.Compare(newFilenames[i], newFilename.Item1, StringComparison.Ordinal) < 0)
                                newFilenames[i] = newFilename.Item1;
                            if (String.Compare(newFilenames[j], newFilename.Item2, StringComparison.Ordinal) < 0)
                                newFilenames[j] = newFilename.Item2;
                        }
                    }
                    else break;
                }

                if (String.Compare(newFilenames[i], groupList[i].Filename, StringComparison.Ordinal) > 0)
                    groupList[i].Filename = newFilenames[i];
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
                throw new ArgumentException("Count of arguments isn't equal to 3!");

            string[] tickets;
            string[] groupsOfStudents;
            var random = new Random();
            var listWithStudents = new List<InfoAboutStudents>();

            if (!Directory.Exists(args[2]))
                throw new ArgumentException("Output directory doesn't exist");
            
            if (Directory.Exists(args[0]))
            {
                tickets = Directory.GetFiles(args[0], "*pdf", SearchOption.AllDirectories);
            }
            else
            {
                throw new ArgumentException("Directory with tickets doesn't exist");
            }

            if (tickets.Length == 0)
                throw new ArgumentException("There is no tickets");

            if (!Directory.Exists(args[1]))
            {
                throw new ArgumentException("Group directory doesn't exist");
            }
            else
            {
                groupsOfStudents = Directory.GetFiles(args[1], "*txt");
                
                if (groupsOfStudents.Length == 0)
                    throw new ArgumentException("No any group of students file");

                foreach (var item in groupsOfStudents)
                {
                    var infoFile = new FileInfo(item);
                    int groupNumber;

                    groupNumber = Convert.ToInt32(infoFile.Name.Substring(0, infoFile.Name.Length - 4));

                    if (groupNumber > 0)
                    {
                        if (infoFile.Length == 0)
                        {
                            Console.WriteLine("Group list {0} is empty!", infoFile.FullName);
                        }
                        else
                        {
                            listWithStudents.Clear();
                            var reader = infoFile.OpenText();

                            while (!reader.EndOfStream)
                                listWithStudents.Add(new InfoAboutStudents(groupNumber, reader.ReadLine()));

                            listWithStudents.Sort((a, b) => a.CompareTo(b));
                            InfoAboutStudents.CheckingStudentsForRepetition(ref listWithStudents);

                            foreach (var student in listWithStudents)
                            {
                                var result = new FileInfo(tickets[random.Next(0, tickets.Length)]);
                                result.CopyTo(args[2] + @"/" + student.Filename);
                            }
                        }
                    }
                }
            }
        }
    }
}