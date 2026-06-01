using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEInHome.Services
{
    public static class Loader
    {
        public static async Task<ObservableCollection<T>> LoadAsync<T>(DbSet<T> dbSet) where T : class
        {
            List<T> result = await dbSet.ToListAsync();
            ObservableCollection<T> collection = new ObservableCollection<T>(result);
            return collection;
        }
    }
}
