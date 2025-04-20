using GraduationProject.data;
using GraduationProject.models;
using GraduationProject.Repository;

namespace GraduationProject.Services
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly AppDBContext _dbContext;
        public IGenaricRepository<User> Users { get; private set; }
        public ICourseRepository Courses { get; private set; }
        public IGenaricRepository<Contactus> Contactus { get; private set; }
        public UnitOfWork(AppDBContext dBContext)
        {
            _dbContext= dBContext;
            Users =new GenaricRepository<User>(dBContext);
            Courses = new CourseRepository(dBContext);
            Contactus = new GenaricRepository<Contactus>(dBContext);
        }
        public int Complete()
        {
           return _dbContext.SaveChanges();
        }
        public async Task<int> CompleteAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
