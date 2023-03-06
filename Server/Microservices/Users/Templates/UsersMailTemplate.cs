using Users.Models;
using Types.Entities;

namespace Users.Templates;

public class UsersMailTemplate
{
    public static string ConfirmEmail(ConfirmEmail confirmEmail)
    {
        return $@"<div>
    <h1>Dear {confirmEmail.User.Username},</h1>
    <hr/>
    <p>
        thanks for signing up with <b>BitStorm</b>! <br/>
        You must follow <a href=""{confirmEmail.Link}"">this link</a> within {UserConstants.ConfirmPeriod.Days} days of registration to activate your account. <br/>
        We are waiting for you! <br/>
        <b>BitStorm team</b>
    </p>
</div>";
    }
}
