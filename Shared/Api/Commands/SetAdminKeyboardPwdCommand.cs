using Shared.Enums;

namespace Shared.Api.Commands;

public class SetAdminKeyboardPwdCommand : AbstractCommand
{
    public const CommandType CommandType = Enums.CommandType.COMM_SET_ADMIN_KEYBOARD_PWD;
    private string _adminPasscode;

    public SetAdminKeyboardPwdCommand(String adminPasscode)
    {
        this._adminPasscode = adminPasscode;
    }

    public SetAdminKeyboardPwdCommand(byte[] data) : base(data)
    {
    }

    public override void ProcessData()
    {
        // Do nothing yet, we don't know if the lock returns anything
        if (Data != null && Data.Length > 0)
        {
            Console.WriteLine("SetAdminKeyboardPwdCommand received: " + BitConverter.ToString(Data));
        }
    }

    public override byte[] Build()
    {
        if (!string.IsNullOrEmpty(_adminPasscode))
        {
            byte[] data = new byte[_adminPasscode.Length];
            for (int i = 0; i < _adminPasscode.Length; i++)
            {
                data[i] = (byte) (_adminPasscode[i] - '0'); // Assumes adminPasscode contains only numeric characters
            }

            return data;
        }
        else
        {
            return [];
        }
    }

    public void SetAdminPasscode(string adminPasscode)
    {
        this._adminPasscode = adminPasscode;
    }

    public string GetAdminPasscode()
    {
        return _adminPasscode;
    }

    public override CommandType GetCommandType()
    {
        return CommandType;
    }
}