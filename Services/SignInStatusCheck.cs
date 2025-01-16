using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;

public class AuthService
{
    private const string AuthTokenFileName = "auth_token.txt"; // File name for the token
    private const string StudentNameFileName = "student_name.txt"; // File name for the student name
    private const string DepartmentFileName = "department.txt"; // File name for the department
    private const string StudentNumberFileName = "student_number.txt"; // File name for the student number
    private bool _isAuthenticated;

    public bool IsAuthenticated() => _isAuthenticated;

    public async Task SignIn(string token, string studentName, string department, string studentNumber)
    {
        _isAuthenticated = true;
        await File.WriteAllTextAsync(Path.Combine(FileSystem.AppDataDirectory, AuthTokenFileName), token);
        await File.WriteAllTextAsync(Path.Combine(FileSystem.AppDataDirectory, StudentNameFileName), studentName);
        await File.WriteAllTextAsync(Path.Combine(FileSystem.AppDataDirectory, DepartmentFileName), department);
        await File.WriteAllTextAsync(Path.Combine(FileSystem.AppDataDirectory, StudentNumberFileName), studentNumber);
    }

    public async Task Logout()
    {
        _isAuthenticated = false;
        var tokenFilePath = Path.Combine(FileSystem.AppDataDirectory, AuthTokenFileName);
        var studentNameFilePath = Path.Combine(FileSystem.AppDataDirectory, StudentNameFileName);
        var departmentFilePath = Path.Combine(FileSystem.AppDataDirectory, DepartmentFileName);
        var studentNumberFilePath = Path.Combine(FileSystem.AppDataDirectory, StudentNumberFileName);

        if (File.Exists(tokenFilePath))
        {
            File.Delete(tokenFilePath);
        }
        if (File.Exists(studentNameFilePath))
        {
            File.Delete(studentNameFilePath);
        }
        if (File.Exists(departmentFilePath))
        {
            File.Delete(departmentFilePath);
        }
        if (File.Exists(studentNumberFilePath))
        {
            File.Delete(studentNumberFilePath);
        }
    }

    public async Task CheckAuthenticationState()
    {
        var tokenFilePath = Path.Combine(FileSystem.AppDataDirectory, AuthTokenFileName);

        if (File.Exists(tokenFilePath))
        {
            var token = await File.ReadAllTextAsync(tokenFilePath);
            _isAuthenticated = !string.IsNullOrEmpty(token);
        }
        else
        {
            _isAuthenticated = false;
        }
    }

    public async Task<string> GetStudentName()
    {
        var studentNameFilePath = Path.Combine(FileSystem.AppDataDirectory, StudentNameFileName);
        return File.Exists(studentNameFilePath) ? await File.ReadAllTextAsync(studentNameFilePath) : null;
    }

    public async Task<string> GetDepartment()
    {
        var departmentFilePath = Path.Combine(FileSystem.AppDataDirectory, DepartmentFileName);
        return File.Exists(departmentFilePath) ? await File.ReadAllTextAsync(departmentFilePath) : null;
    }

    public async Task<string> GetStudentNumber()
    {
        var studentNumberFilePath = Path.Combine(FileSystem.AppDataDirectory, StudentNumberFileName);
        return File.Exists(studentNumberFilePath) ? await File.ReadAllTextAsync(studentNumberFilePath) : null;
    }
}