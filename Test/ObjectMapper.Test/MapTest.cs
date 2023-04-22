using System.Text.Json;

namespace ObjectMapper.Test;

[TestClass]
public class MapTest
    {
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public void MapObjectTest()
        {

            var student = new Student
            {
                Id = 1,
                Name = "Jaxongir Umarov",
                Age = 20
            };

            var updateStudent = new StudentDto
            {
                FirstName = "Jahongir",
                LastName = "Umarov",
                Age = 21
            };

            TestContext?.WriteLine("Before");
            TestContext?.WriteLine(JsonSerializer.Serialize(student));
            TestContext?.WriteLine(JsonSerializer.Serialize(updateStudent));

            MapObject<StudentDto, Student>.GetMapObject()
                .CustomMap(dest => dest.Name, src => $"{src.FirstName} {src.LastName}")
                .Ignore(dest => dest.Id)
                .Copy(updateStudent, student);

            TestContext?.WriteLine("After");
            TestContext?.WriteLine(JsonSerializer.Serialize(student));
            TestContext?.WriteLine(JsonSerializer.Serialize(updateStudent));

        }
    }

    class Student
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }
    }

    class StudentDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int Age { get; set; }
    }