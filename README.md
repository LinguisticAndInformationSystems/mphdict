# mphdict
Digital lexicographic systems Ukrainian language + (the grammatical dictionary, synonymous dictionary, etymological dictionary +)

Copyright © 2016-2021 uSofTrod. Contacts: uSofTrod@outlook.com

License: http://opensource.org/licenses/MIT

Цифрові лексикографічні системи української мови + (серія "Цифрове лексикографічне надбання України")

Проект містить SQLite бази даних граматичних словників, що зберігають інформацію про слова, всі словозмінні класи, властиві їм квазіфлексії, акцентуаційні класи та інші дані (первинним джерелом лінгвістичної інформації для граматичного словника української мови в першу чергу слугував "Орфографічний словник української мови" С. Головащука, М. Пещак, В. Русанівського, О. Тараненка, створений на основі 4-го видання "Українського правопису" (1993). Для БД граматичного словника російської мови - «Грамматический словарь русского языка» А. А. Залізняка), бази даних словників синонімів, базу даних етимологічного словника української мови, що була сформована в освітніх цілях на основі томів 1–6 Етимологічного словника української мови Інституту мовознавства ім. О.О. Потебні НАН України: була розроблена оригінальна структура БД та розроблена веб-система її відображення. Ми не надаємо доступ до твору Етимологічний словник української мови Інституту мовознавства ім. О.О. Потебні НАН України, а в освітніх цілях демонструємо можливість роботи зі складноструктурованою БД етимологічної системи, яка є нашою оригінальною розробкою. Також проект містить бібліотеку роботи з базами (mphdict project) та веб-систему словників (mphweb project).

Детальніше

https://github.com/LinguisticAndInformationSystems/mphdict/wiki

Чому наша система, що є у нас і чого немає в жодному іншому українському словнику - три головні речі + 1: 
1. Ми описали внутрішню структуру бази даних словника і перші відкрили алгоритми роботи з даними. 
2. Ми створили і надали вільний доступ до словника словозмінної класифікації. Хочемо зауважити, що жоден ресурс не надає словозмінної та акцентуаційної класифікації. Відповідно це є основним бар'єром у використанні цих ресурсів в основі систем аналізу тексту. І саме це спонукало розробку відкритого словника. 
3. Реєстр нашого граматичного словника української мови складає 261499 слів (на вересень 2016 року). І на цей час, наскільки нам відомо, це найбільший словник такого типу.
4. Ми надаємо доступ до даних та алгоритмів а не до сервісів. 

Ми на Фейсбук

https://www.facebook.com/USofTrod-180689769033511/

Required software

.NET Core 2.1

https://github.com/dotnet/core/

ASP.NET Core 2.1

https://github.com/aspnet/home

How to start

https://www.microsoft.com/net/core/platform

База даних "mph_ua.db" доступна під ліцензією Open Database License http://opendatacommons.org/licenses/odbl/1.0/. Будь-які права на вміст (контент) цієї бази даних ліцензовано під ліцензією Database Contents License https://opendatacommons.org/licenses/dbcl/1.0/.

База даних "mph_ru.db" доступна під ліцензією Open Database License http://opendatacommons.org/licenses/odbl/1.0/. Будь-які права на вміст (контент) цієї бази даних ліцензовано під ліцензією Database Contents License https://opendatacommons.org/licenses/dbcl/1.0/.

База даних "synsets_ua.db" доступна під ліцензією Open Database License http://opendatacommons.org/licenses/odbl/1.0/. Будь-які права на вміст (контент) цієї бази даних ліцензовано під ліцензією Database Contents License https://opendatacommons.org/licenses/dbcl/1.0/.

База даних "synsets_ru.db" доступна під ліцензією Open Database License http://opendatacommons.org/licenses/odbl/1.0/. Будь-які права на вміст (контент) цієї бази даних ліцензовано під ліцензією Database Contents License https://opendatacommons.org/licenses/dbcl/1.0/.

База даних "etym.db" доступна під ліцензією Open Database License http://opendatacommons.org/licenses/odbl/1.0/. Будь-які права на вміст (контент) цієї бази даних ліцензовано під ліцензією Database Contents License https://opendatacommons.org/licenses/dbcl/1.0/.

Warranty Disclaimer: No Warranty!

IN NO EVENT SHALL THE AUTHOR, OR ANY OTHER PARTY WHO MAY MODIFY AND/OR REDISTRIBUTE THIS PROGRAM AND DOCUMENTATION, BE LIABLE FOR ANY COMMERCIAL, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES ARISING OUT OF THE USE OR INABILITY TO USE THE PROGRAM INCLUDING, BUT NOT LIMITED TO, LOSS OF DATA OR DATA BEING RENDERED INACCURATE OR LOSSES SUSTAINED BY YOU OR LOSSES SUSTAINED BY THIRD PARTIES OR A FAILURE OF THE PROGRAM TO OPERATE WITH ANY OTHER PROGRAMS, EVEN IF YOU OR OTHER PARTIES HAVE BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.
