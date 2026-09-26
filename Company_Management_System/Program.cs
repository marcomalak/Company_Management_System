using Company_Management_System.Models;
using EmployeeProjectManagement.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace Company_Management_System
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<string> list = new List<string>() { "---- The Menu ----", "  ADD  ", "  SHOW  ","  Edit  ","  Delete  ", "  Exit  " };
            while (true)
            {
                int choice = Menu(list);
                switch (choice)
                {
                    case 0:
                        AddMenu();
                        break;
                    case 1:
                        ShowMenu();
                        break;
                    case 2:
                        EditMenu();
                        break;
                    case 3:
                        DeleteMenu();
                        break;
                    case -1:
                        Console.ResetColor();
                        Console.Clear();
                        return;
                }
                Console.ResetColor();
                Console.Write("\x1b[2J\x1b[3J\x1b[H");
            }
        }

        public static void AddDep()
        {
            using var context = new CompanyDbContext();
            Department dep = new Department();
            Console.Write("Enter the name of the new Department : ");
            string Name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(Name)) { Console.WriteLine("\nCan`t Add Department without name !");
                return;
            }
            else if (context.Departments.Any(d => d.Name == Name))
            {
                Console.WriteLine("\nthis department is already exist!");
                return;
            }
            dep.Name = Name;
            context.Departments.Add(dep);
            context.SaveChanges();
            Console.WriteLine();
        }
        public static void AddEmp()
        {
            using var context = new CompanyDbContext();
            Employee emp = new Employee();
            var deps= context.Departments.ToList();
            string fname, lname;
            Console.WriteLine("Write 0 then press Enter to cancel :)");
            while(true) {
                Console.Write("Enter the first name of new Employee : ");
                fname = Console.ReadLine();
                if (fname.ElementAtOrDefault(0) == '0') return;
                Console.Write("Enter the last name of new Employee : ");
                lname = Console.ReadLine();
                if (lname.ElementAtOrDefault(0) == '0') return;
                if (string.IsNullOrWhiteSpace(fname) || string.IsNullOrWhiteSpace(lname))
                {
                    Console.WriteLine("Can`t Add Employee without name !");
                }
                else break;
            }
            int id = -1;
            bool check;
            while (true)
            {
                for (int i = 0; i < deps.Count; i++)
                {
                    Console.WriteLine((i + 1) + ") " + deps[i].Name);
                }
                Console.WriteLine($"{deps.Count+1}) No Department");
                Console.Write("select your department number from the list : ");
                check = int.TryParse(Console.ReadLine(), out id);
                if (id == 0 && check){
                    Console.WriteLine("\nAdding has canceled!\n");
                    return; }
                else if (id > 0 && id <= deps.Count) { emp.DepartmentId = deps[id - 1].DepartmentId; break; }
                else if (id == deps.Count+1)
                {
                    emp.DepartmentId = null;
                    break;
                }
                else
                    Console.WriteLine("\nInvalid Choice!\n");
            }
            emp.FirstName = fname;
            emp.LastName = lname;
            
            context.Employees.Add(emp);
            context.SaveChanges();
            Console.WriteLine();
        }
        public static void AddProject()
        {
            using var context = new CompanyDbContext();
            Project project = new Project();
            Console.Write("Enter the name of new Project : ");
            string Name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(Name))
            {
                Console.WriteLine("\nCan`t Add Project without name !");
                return;
            }
            bool check;
            Console.Write("Enter Start date : ");
            check= DateOnly.TryParse(Console.ReadLine(),out DateOnly date);
            if (check)
                project.StartDate = date;
            else
                project.StartDate = DateOnly.FromDateTime(DateTime.Now);
            Console.Write("Enter End date : ");
            check = DateOnly.TryParse(Console.ReadLine(), out date);
            if (check)
                project.EndDate = date;
            else
                project.EndDate = null;
            context.Projects.Add(project);
            context.SaveChanges();
            Console.WriteLine();
        }

        public static void ShowDeps()
        {
            using var context = new CompanyDbContext();
            var deps=context.Departments.Include(e => e.Employees).ToList();
            Console.WriteLine("-----------Departments-----------");
            Console.WriteLine("==================================\n");
                int counterD = 1;
            int counterE=1;
            foreach (var dep in deps)
            {
                counterE = 1;
                Console.WriteLine($"{counterD}) {dep.Name}");
                    counterD++;
                foreach( var emp in dep.Employees)
                {
                        Console.WriteLine($"\t{counterE}) {emp.FirstName} {emp.LastName}");
                        counterE++;
                }
                if (counterE == 1)
                {
                    Console.WriteLine("\tNo Employees");
                    Console.WriteLine();
                }
                Console.WriteLine();
                Console.WriteLine(new string('-', 50));
                Console.WriteLine();
            }
        }
        public static void ShowEmps()
        {
            using var context = new CompanyDbContext();
            int counterE = 1;
            var emps = context.Employees.Include(e => e.Projects).Include(e => e.Department).ToList();
            Console.WriteLine("-----------Employees-----------");
            Console.WriteLine("================================\n");
            foreach (var emp in emps)
            {
                int counterP = 1;
                if (emp.Department==null)
                    Console.WriteLine($"{counterE}) Name : {emp.FirstName} {emp.LastName} | Department Name : None ");
                else
                    Console.WriteLine($"{counterE}) Name : {emp.FirstName} {emp.LastName} | Department Name : {emp.Department.Name} ");
                counterE++;
                Console.WriteLine("Projects : ");
                foreach(var pro in emp.Projects)
                {
                    Console.WriteLine($"\t{counterP}) {pro.Name} => Start in: {pro.StartDate} => End in: {pro.EndDate}");
                    counterP++;
                }
                if (counterP == 1) Console.WriteLine("\tNo Projects");
                Console.WriteLine();
                Console.WriteLine(new string('-', 50));
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        public static void ShowProjects()
        {
            using var context = new CompanyDbContext();
            var projects = context.Projects.Include(e => e.Employees).ToList();
            int counterE = 1;
            int counerP = 1;
            Console.WriteLine("-----------Projects-----------");
            Console.WriteLine("===============================\n");
            foreach (var project in projects)
            {
                counterE = 1;
                Console.WriteLine($"{counerP}) Name : {project.Name} | Start Date : {project.StartDate} | End Date : {project.EndDate}");
                counerP++;
                Console.WriteLine("Employees : ");
                foreach(var emp in project.Employees){
                    Console.WriteLine($"\t{counterE}) {emp.FirstName} {emp.LastName}");
                    counterE++;
                }
                if (counterE == 1) Console.WriteLine("\tNo Employees");
                Console.WriteLine();
                Console.WriteLine(new string('-', 50));
                Console.WriteLine();
            }
            Console.WriteLine();
        }


        public static void EditDep()
        {
            using var context = new CompanyDbContext();
            Console.WriteLine("Which department you want to Edit ?");
            var deps=context.Departments.Include(e=>e.Employees).ToList();
            int counter = 1;
            int idD = -1;
            foreach (var dep in deps)
            {
                Console.WriteLine($"{counter}) {dep.Name}");
                counter++;
            }
            Console.Write("your Choice : ");
            int.TryParse( Console.ReadLine(), out idD);
            if (idD > 0 && idD <= deps.Count)
            {
                counter = 1;
                Console.WriteLine($"Department Name : {deps[idD-1].Name}");
                foreach (var emp in deps[idD-1].Employees)
                {
                    Console.WriteLine($"\t{counter}) {emp.FirstName} {emp.LastName}");
                    counter++;
                }
                Console.WriteLine("\nWhat do you want to Edit?\n1) Department Name\n2) Assign new employee\n3) Cancel");
                Console.Write("your Choice : ");
                int.TryParse(Console.ReadLine(), out counter);
                switch (counter) {
                    case 1:
                        Console.Write("what is the new name ?\nThe new name : ");
                        string name= Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(name)) Console.WriteLine("Can`t Make the department with No Name!");
                        else
                        {
                            deps[idD - 1].Name = name;
                            context.SaveChanges();
                        }
                        break;
                    case 2:
                        var emps2=context.Employees.Where(e=>e.DepartmentId==null).ToList();
                        if (emps2.Count == 0) Console.WriteLine("There is`t any free Employee to Assign!");
                        else
                        {
                            bool flag = true;
                            bool check = true;
                            int id = 1;
                            while (flag)
                            {
                                emps2 = context.Employees.Where(e => e.DepartmentId == null).ToList();
                                id = 1;
                                foreach(var emp in emps2)
                                {
                                    Console.WriteLine($"{id})Employee Name : {emp.FirstName} {emp.LastName}");
                                    id++;
                                }
                                Console.Write("select who you want to add (write 0 to cancel) : ");
                                check= int.TryParse (Console.ReadLine(), out id);
                                if (id > emps2.Count&&check)
                                {
                                    Console.WriteLine("Invalid Choice!");
                                    continue;
                                }
                                else if (id == 0 && check)
                                {
                                    Console.WriteLine("the Editing has canceled");
                                    return;
                                }
                                else if (id>0&&id<=emps2.Count)
                                {
                                    emps2[id - 1].DepartmentId = deps[idD - 1].DepartmentId;
                                }
                                else
                                {
                                    Console.WriteLine("Invalid Choice!");
                                    continue;
                                }
                                context.SaveChanges();
                                Console.WriteLine("Do you want to add more ?(y/n)");
                                string c= Console.ReadLine();
                                if (string.IsNullOrWhiteSpace(c)||!c.StartsWith('y'))
                                {
                                    flag = false;
                                }
                            }
                        }
                        break;
                    default:
                        Console.WriteLine("the Editing has canceled");
                        return;
                }
            }
            else
            {
                Console.WriteLine("Invalid Choice !");
            }
        }
        public static void EditEmp()
        {
            using var context = new CompanyDbContext();
            var emps =context.Employees.Include(e=>e.Projects).Include(e=>e.Department).ToList();
            Console.WriteLine("Which Employee you want to Edit ?");
            int Eid = 1;
            int Pid = 1;
            foreach (var emp in emps) {
                if (emp.Department == null)
                    Console.WriteLine($"{Eid}) Name : {emp.FirstName} {emp.LastName} | Department Name : None ");
                else
                    Console.WriteLine($"{Eid}) Name : {emp.FirstName} {emp.LastName} | Department Name : {emp.Department.Name} ");
                Eid++;
                foreach (var pro in emp.Projects)
                {
                    Console.WriteLine($"\t{Pid}) {pro.Name} => Start in: {pro.StartDate} => End in: {pro.EndDate}");
                    Pid++;
                }
                Pid = 1;
            }
            Console.Write("your Choice : ");
            int.TryParse(Console.ReadLine(), out Eid);
            if (Eid > 0 && Eid <= emps.Count) {
                int choice = 0;
                if (emps[Eid - 1].Department == null)
                    Console.WriteLine($"{Eid}) Name : {emps[Eid-1].FirstName} {emps[Eid - 1].LastName} | Department Name : None ");
                else
                    Console.WriteLine($"{Eid}) Name : {emps[Eid - 1].FirstName} {emps[Eid - 1].LastName} | Department Name : {emps[Eid - 1].Department.Name} ");
                foreach (var pro in emps[Eid - 1].Projects)
                {
                    Console.WriteLine($"\t{Pid}) {pro.Name} => Start in: {pro.StartDate} => End in: {pro.EndDate}");
                }
                Console.Write("\nWhat do you want to Edit?\n1) Name\n2) Department\n3) Projects\nYour choice : ");
                int.TryParse (Console.ReadLine(), out choice);
                switch (choice) {
                    case 1:
                        string fname = "", lname = "";
                        Console.Write("The New first name : ");
                        fname=Console.ReadLine();
                        Console.Write("The New last name : ");
                        lname = Console.ReadLine();
                        if(string.IsNullOrWhiteSpace(fname) || string.IsNullOrWhiteSpace(lname))
                        {
                            Console.WriteLine("Can`t Edit Employee Name to Null name !\nThe Editing has canceled.\n\n");
                            return;
                        }
                        emps[Eid - 1].FirstName = fname;
                        emps[Eid - 1].LastName = lname;
                        context.SaveChanges();
                        break;
                    case 2:
                        Console.WriteLine($"Current Department : {emps[Eid-1].Department.Name}\n");
                        var deps = context.Departments.ToList();
                        Console.WriteLine("Valid Departments : ");
                        int counter = 1;
                        foreach (var dep in deps)
                        {
                            Console.WriteLine($"{counter}) {dep.Name}");
                            counter++;
                        }
                        Console.WriteLine($"{counter}) No Department");
                        int c = 0;
                        Console.Write("Your Choice : ");
                        int.TryParse(Console.ReadLine(), out c);
                        if (c > 0 && c <= deps.Count)
                        {
                            emps[Eid - 1].DepartmentId = deps[c - 1].DepartmentId;
                            context.SaveChanges();
                        }
                        else if (c == counter)
                        {
                            emps[Eid - 1].DepartmentId = null;
                            context.SaveChanges();
                        }
                        else
                            Console.WriteLine("Invalid Choice!\nThe Editing has canceled.\n\n");
                        break;
                    case 3:
                        Console.Write("What Do you want to do?\n1) Assign new project\n2)Remove from project\nYour Choice : ");
                        int c2 = -1;
                        int.TryParse(Console.ReadLine(), out c2);
                        switch (c2) {
                            case 1:
                                var projects = context.Projects.Where(p => !p.Employees.Any(e => e.EmployeeId == emps[Eid - 1].EmployeeId)).ToList();
                                Pid = 1;
                                Console.WriteLine("Valid Projects : ");
                                foreach (var project in projects)
                                {
                                    Console.WriteLine($"{Pid}) {project.Name} => Start Date : {project.StartDate} => End Date : {project.EndDate}");
                                    Pid++;
                                }
                                Console.Write("Which project you want to Assign in :");
                                int.TryParse(Console.ReadLine(), out Pid);
                                if(Pid>0&&Pid<=projects.Count) {
                                    emps[Eid-1].Projects.Add(projects[Pid-1]);
                                    context.SaveChanges();
                                }
                                else
                                    Console.WriteLine("Invalid Choice!\nThe Editing has canceled.\n\n");
                                break;
                            case 2:
                                var projects2 = context.Projects.Where(p => p.Employees.Any(e => e.EmployeeId == emps[Eid - 1].EmployeeId)).ToList();
                                Pid = 1;
                                Console.WriteLine("Valid Projects : ");
                                foreach (var project in projects2)
                                {
                                    Console.WriteLine($"{Pid}) {project.Name} => Start Date : {project.StartDate} => End Date : {project.EndDate}");
                                    Pid++;
                                }
                                Console.Write("Which project you want to Remove :");
                                int.TryParse(Console.ReadLine(), out Pid);
                                if (Pid > 0 && Pid <= projects2.Count)
                                {
                                    emps[Eid - 1].Projects.Remove(projects2[Pid - 1]);
                                    context.SaveChanges();
                                }
                                else
                                    Console.WriteLine("Invalid Choice!\nThe Editing has canceled.\n\n");
                                break;
                            default:
                                Console.WriteLine("Invalid Choice!\nThe Editing has canceled.\n\n");
                                return;
                        }
                        break;
                    default: Console.WriteLine("Invalid Choice!\nThe Editing has canceled.\n\n");
                        return;
                }
            }
            else
            {
                Console.WriteLine("invalid Choice!");
            }
        }
        public static void EditProject()
        {
            using var context = new CompanyDbContext();
            var projects =context.Projects.Include(p => p.Employees).ThenInclude(e => e.Department).ToList();
            int Pid = 1;
            Console.WriteLine("Which Project you want to Edit ?");
            foreach (var project in projects) {
                Console.WriteLine($"{Pid}) {project.Name} => Start in: {project.StartDate} => End in: {project.EndDate}");
                Pid++;
            }
            Console.Write("Your Choice : ");
            int.TryParse (Console.ReadLine(), out Pid);
            if (Pid > 0 && Pid <= projects.Count) {
                int Eid = 1;
                Console.WriteLine($"{projects[Pid-1].Name} => Start in: {projects[Pid - 1].StartDate} => End in: {projects[Pid - 1].EndDate}");
                Console.WriteLine("Employees : ");
                if (projects[Pid - 1].Employees.Count == 0)
                {
                    Console.WriteLine("\tthere is No Employees work on this Project\n");
                }
                else
                {
                    foreach (var emp in projects[Pid - 1].Employees)
                    {
                        if (emp.Department == null)
                            Console.WriteLine($"\t{Eid}) Name : {emp.FirstName} {emp.LastName} | Department Name : None ");
                        else
                            Console.WriteLine($"\t{Eid}) Name : {emp.FirstName} {emp.LastName} | Department Name : {emp.Department.Name} ");
                        Eid++;
                    }
                }
                Console.Write("What do you want to edit?\n1) project Name\n2) Start date\n3) End date\n4) Assign Employee to the project\nYour choice : ");
                int choice = 0;
                int.TryParse (Console.ReadLine(), out choice);
                switch (choice) {
                    case 1:
                        Console.Write("what is the new name ?\nThe new name : ");
                        string name = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(name)) Console.WriteLine("Can`t Make the Project with No Name!");
                        else
                        {
                            projects[Pid - 1].Name = name;
                            context.SaveChanges();
                        }
                        break;
                    case 2:
                        bool check;
                        Console.Write("Enter new Start date : ");
                        check = DateOnly.TryParse(Console.ReadLine(), out DateOnly date);
                        if (check)
                            projects[Pid-1].StartDate = date;
                        else
                            { Console.WriteLine("Invalid Date!\nThe Editing has canceled.\n\n");return; }
                        break;
                    case 3:
                        Console.Write("Enter new End date : ");
                        check = DateOnly.TryParse(Console.ReadLine(), out date);
                        if (check)
                            projects[Pid - 1].EndDate = date;
                        else
                        { Console.WriteLine("Invalid Date!\nThe Editing has canceled.\n\n"); return; }
                        break;
                    case 4:
                        var emps2 = context.Employees.Where(e => !e.Projects.Any(p => p.ProjectId == projects[Pid-1].ProjectId)).ToList();
                        Console.WriteLine("Valid Employees : ");
                        Eid = 1;
                        foreach(var emp in emps2)
                        {
                            if (emp.Department == null)
                                Console.WriteLine($"\t{Eid}) Name : {emp.FirstName} {emp.LastName} | Department Name : None ");
                            else
                                Console.WriteLine($"\t{Eid}) Name : {emp.FirstName} {emp.LastName} | Department Name : {emp.Department.Name} ");
                            Eid++;
                        }
                        Console.Write("Your Choice : ");
                        int.TryParse(Console.ReadLine(), out Eid);
                        if (Eid > 0&& Eid <= emps2.Count)
                        {
                            projects[Pid-1].Employees.Add(emps2[Eid-1]);
                            context.SaveChanges();
                        }
                        else
                            Console.WriteLine("Invalid Choice!\nThe Editing has canceled.\n\n");
                        break;
                    default:
                        Console.WriteLine("Invalid Choice!\nThe Editing has canceled.\n\n");
                        return;
                }
            }
            else
                Console.WriteLine("Invalid Choice!\nThe Editing has canceled.\n\n");

        }

        public static void DeleteDep()
        {
            using var context = new CompanyDbContext();
            var deps = context.Departments.ToList();
            Console.WriteLine("Which Department you want to delete ?\n");
            int Did = 1;
            foreach(var dep in deps)
            {
                Console.WriteLine($"{Did}) {dep.Name}");
                Did++;
            }
            Console.Write("Your Choice : ");
            int.TryParse(Console.ReadLine(), out Did);
            if (Did > 0 && Did <= deps.Count)
            {
                context.Departments.Remove(context.Departments.Find(deps[Did-1].DepartmentId));
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("Invalid Choice!\nThe Deleting has canceled.\n\n");
            }
        }
        public static void DeleteEmp()
        {
            using var context = new CompanyDbContext();
            var emps = context.Employees.ToList();
            Console.WriteLine("Which Employee you want to delete ?\n");
            int Eid = 1;
            foreach (var emp in emps)
            {
                Console.WriteLine($"{Eid}) {emp.FirstName} {emp.LastName}");
                Eid++;
            }
            Console.Write("Your Choice : ");
            int.TryParse(Console.ReadLine(), out Eid);
            if (Eid > 0 && Eid <= emps.Count)
            {
                context.Employees.Remove(context.Employees.Find(emps[Eid - 1].EmployeeId));
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("Invalid Choice!\nThe Deleting has canceled.\n\n");
            }
        }
        public static void DeleteProject()
        {
            using var context = new CompanyDbContext();
            var projects = context.Projects.ToList();
            Console.WriteLine("Which Project you want to delete ?\n");
            int Pid = 1;
            foreach (var project in projects)
            {
                Console.WriteLine($"{Pid}) {project.Name}");
                Pid++;
            }
            Console.Write("Your Choice : ");
            int.TryParse(Console.ReadLine(), out Pid);
            if (Pid > 0 && Pid <= projects.Count)
            {
                context.Projects.Remove(context.Projects.Find(projects[Pid - 1].ProjectId));
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("Invalid Choice!\nThe Deleting has canceled.\n\n");
            }
        }

        public static int Menu(List<string> menu)
        {
            int x_dest = Console.WindowWidth / 2;
            int y_dest = Console.WindowHeight / (menu.Count + 1);
            int highlightIndex = 1;
            ConsoleKeyInfo key;
            List<Employee> emps = new List<Employee>();
            do
            {
                for (int i = 0; i < menu.Count; i++)
                {
                    if (i == highlightIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    Console.SetCursorPosition(x_dest - (menu[i].Length / 2), y_dest * (i + 1));
                    Console.Write(menu[i]);
                }
                Console.WriteLine();
                key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        highlightIndex--;
                        if (highlightIndex < 1)
                        {
                            highlightIndex = menu.Count - 1;
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        highlightIndex++;
                        if (highlightIndex > menu.Count - 1)
                        {
                            highlightIndex = 1;
                        }
                        break;
                    case ConsoleKey.Enter:
                        Console.Clear();
                        if (highlightIndex == menu.Count - 1)
                            return -1;
                        else
                        {
                            Console.ResetColor();
                            return highlightIndex-1;
                        }
                }
            } while (true);
        }
        public static void AddMenu()
        {
            Console.WriteLine();
            int choice = -2;
            List<string> menu = new List<string>() {"---- Add Menu ----","  Employee  ","  Department  ","  Project  ","  Back  " };
            while (true)
            {
                choice = Menu(menu);
                switch (choice)
                {
                    case 0:
                        AddEmp();                    break;
                    case 1:
                        AddDep();
                        break;
                    case 2:
                        AddProject();
                        break;
                    case -1:
                        return;
                }
                Console.WriteLine("\nEnter any key to back to the Menu....");
                Console.ReadKey();
                Console.ResetColor();
                Console.Write("\x1b[2J\x1b[3J\x1b[H");
            }
        }
        public static void ShowMenu()
        {
            int choice = -2;
            List<string> menu = new List<string>() { "---- Show Menu ----", "  Employee  ", "  Department  ", "  Project  ", "  Back  " };
            while (true)
            {
                choice = Menu(menu);
                switch (choice)
                {
                    case 0:
                        ShowEmps();
                        break;
                    case 1:
                        ShowDeps();
                        break;
                    case 2:
                        ShowProjects();
                        break;
                    case -1:

                        Console.ResetColor();
                        Console.Clear();
                        return;
                }
                Console.WriteLine("\nEnter any key to back to the Menu....");
                Console.ReadKey();
                Console.ResetColor();
                Console.Write("\x1b[2J\x1b[3J\x1b[H");
            }
        }
        public static void EditMenu()
        {
            int choice = -2;
            List<string> menu = new List<string>() { "---- Edit Menu ----", "  Employee  ", "  Department  ", "  Project  ", "  Back  " };
            while (true)
            {
                choice = Menu(menu);
                switch (choice)
                {
                    case 0:
                        EditEmp();
                        break;
                    case 1:
                        EditDep();
                        break;
                    case 2:
                        EditProject();
                        break;
                    case -1:
                        return;
                }
                Console.WriteLine("\nEnter any key to back to the Menu....");
                Console.ReadKey();
                Console.ResetColor();
                Console.Write("\x1b[2J\x1b[3J\x1b[H");
            }
        }
        public static void DeleteMenu()
        {
            int choice = -2;
            List<string> menu = new List<string>() { "---- Delete Menu ----", "  Employee  ", "  Department  ", "  Project  ", "  Back  " };
            while (true)
            {
                choice = Menu(menu);
                switch (choice)
                {
                    case 0:
                        DeleteEmp();
                        break;
                    case 1:
                        DeleteDep();
                        break;
                    case 2:
                        DeleteProject();
                        break;
                    case -1:
                        return;
                }
                Console.WriteLine("\nEnter any key to back to the Menu....");
                Console.ReadKey();
                Console.ResetColor();
                Console.Write("\x1b[2J\x1b[3J\x1b[H");
            }
        }
    }
}
