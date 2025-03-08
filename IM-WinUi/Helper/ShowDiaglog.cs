using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace IMWinUi.Helper
{
    public class ShowDiaglog
    {
        // 引入 MessageBox 函数
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

        // 定义 MessageBox 的类型常量
        private const uint MB_OK = 0x0; // 确定按钮
        private const uint MB_OKCANCEL = 0x1; // 确定和取消按钮
        private const uint MB_YESNO = 0x4; // 是和否按钮

        // 显示消息框的方法
        public static Task<bool> ShowMessage(string title, string content)
        {
            // 调用 MessageBox 函数
            MessageBox(IntPtr.Zero, content, title, MB_OK);
            return Task.FromResult(true);
        }

    }
}

