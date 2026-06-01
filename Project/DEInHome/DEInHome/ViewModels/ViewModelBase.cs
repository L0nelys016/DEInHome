using CommunityToolkit.Mvvm.ComponentModel;
using DEInHome.Models;

namespace DEInHome.ViewModels
{
    public abstract class ViewModelBase : ObservableObject
    {
        protected VolkovContext _db = new();

        protected static User? _loginedUser;
    }
}
