namespace GalloFlix.Helpers;

public static class TranslateIdentityErrors
{
    public static string TranslateErrorMessage(string codeError)
    {
        string message = string.Empty;
        switch (codeError)
        {
            case "DefaultError":
                message = "Ocorreu um erro desconhecido.";
                break;
            case "ConcurrencyFailure":
                message = "Falha de concorrência otimista, o objeto foi modificado.";
                break;
            case "InvalidToken":
                message = "Token inválido.";
                break;
            case "LoginAlreadyAssociated":
                message = "Já existe um usuário com este login.";
                break;
            case "InvalidUserName":
                message = $"Este login é inválido, um login deve conter apenas letras ou dígitos.";
                break;
            case "InvalidEmail":
                message = "E-mail inválido.";
                break;
            case "DuplicateUserName":
                message = "Este login já está sendo utilizado.";
                break;
            case "DuplicateEmail":
                message = $"Este email já está sendo utilizado.";
                break;
            case "InvalidRoleName":
                message = "Esta permissão é inválida.";
                break;
            case "DuplicateRoleName":
                message = "Esta permissão já está sendo Utilizada";
                break;
            case "UserAlreadyInRole":
                message = "Usuário já possui esta permissão.";
                break;
            case "UserNotInRole":
                message = "Usuário não tem esta permissão.";
                break;
            case "UserLockoutNotEnabled":
                message = "Lockout não está habilitado para este usuário.";
                break;
            case "UserAlreadyHasPassword":
                message = "Usuário já possui uma senha definida.";
                break;
            case "PasswordMismatch":
                message = "Senha incorreta.";
                break;
            case "PasswordTooShort":
                message = "Senha muito curta.";
                break;
            case "PasswordRequiresNonAlphanumeric":
                message = "Senhas devem conter ao menos um caracter não alfanumérico.";
                break;
            case "PasswordRequiresDigit":
                message = "Senhas devem conter ao menos um digito ('0'-'9').";
                break;
            case "PasswordRequiresLower":
                message = "Senhas devem conter ao menos um caracter em caixa baixa ('a'-'z').";
                break;
            case "PasswordRequiresUpper":
                message = "Senhas devem conter ao menos um caracter em caixa alta ('A'-'Z').";
                break;
            default:
                message = "Ocorreu um erro desconhecido.";
                break;
        }
        return message;
    }
}