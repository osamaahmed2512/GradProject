using GraduationProject.models;

namespace GraduationProject.Services
{
    public interface IUnitOfWork : IDisposable
    {
        public IGenaricRepository<User> Users { get;  }
        public ICourseRepository Courses { get; }
        IGenaricRepository<Contactus> Contactus { get; }
        int Complete();
        Task<int> CompleteAsync();
    }
}
