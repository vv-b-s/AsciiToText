# AsciiToText
Android application

This application gets plain text and converts it to Ascii in base 2,8,10 and 16.
The same can be done backwards by typing Ascii code in the selected base. The app simply converts integers to chars and sticks them together.

To separate one integer from another use spaces. (String of text with no spaces will result in translating only one character)
The statement above doesn't count for Binary Ascii. Text can be written with spaces or withot them, the output will be the same - the string is divided by 8 and each part is translated to the corresponding character.
This also means that not all text can be translated in binary as the maximum number contained in 8 bits is 255 which is equal to the character 'ÿ'.
Other bases will translate input fine.

Use the take a photo button to snap a picture and extract the text from it.
