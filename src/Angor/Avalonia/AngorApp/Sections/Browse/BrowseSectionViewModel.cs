using System.Reactive.Linq;
using Angor.UI.Model;
using Angor.Wallet.Application;
using AngorApp.Sections.Browse.ProjectLookup;
using AngorApp.UI.Services;
using CSharpFunctionalExtensions;
using ReactiveUI.SourceGenerators;
using Zafiro.Avalonia.Controls.Navigation;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.Reactive;

namespace AngorApp.Sections.Browse;

public partial class BrowseSectionViewModel : ReactiveObject, IBrowseSectionViewModel
{
    [Reactive] private string? projectId;

    [ObservableAsProperty] private IList<IProjectViewModel>? projects;

    public BrowseSectionViewModel(IWalletAppService walletAppService, IProjectService projectService, INavigator navigator,
        UIServices uiServices)
    {
        ProjectLookupViewModel = new ProjectLookupViewModel(projectService, walletAppService, navigator, uiServices);

        LoadLatestProjects = ReactiveCommand.CreateFromObservable(() => Observable.FromAsync(projectService.Latest)
            .Flatten()
            .Select(IProjectViewModel (project) => new ProjectViewModel(walletAppService, project, navigator, uiServices))
            .ToList());

        OpenHub = ReactiveCommand.CreateFromTask(() =>
            uiServices.LauncherService.LaunchUri(new Uri("https://www.angor.io")));
        projectsHelper = LoadLatestProjects.ToProperty(this, x => x.Projects);
        LoadLatestProjects.Execute().Subscribe();
    }

    public IProjectLookupViewModel ProjectLookupViewModel { get; }

    public ReactiveCommand<Unit, IList<IProjectViewModel>> LoadLatestProjects { get; }

    public ReactiveCommand<Unit, Unit> OpenHub { get; set; }
}