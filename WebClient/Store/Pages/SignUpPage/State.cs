using Fluxor;
using WebClient.Store.Common;
using WebClient.Typing;

namespace WebClient.Store.Pages.SignUpPage;

[FeatureState]
public record SignUpPageState: BaseUxServerErrorState<SignUpPageState>
{
    public override UxState InitialState => UxState.Editable;
}
