using Fluxor;
using WebClient.Store.Common;
using WebClient.Typing;

namespace WebClient.Store.Pages.SignInPage;

[FeatureState]
public record SignInPageState: BaseUxServerErrorState<SignInPageState>
{
    public override UxState InitialState => UxState.Editable;
}
