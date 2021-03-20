# GtbToolsForRevit

Zbior funkcji wspierajacych projektowanie.

Opis wybranych:

1. ExternalLinkTool
Program do szybkiego kontrolowania widocznosci linkow zewnetrznych rvt i cad na rzutach.

2. CutOpeningMemory
Program zapisuje aktualna pozycje i wymiary otworow (face based generic models typu void) do ich extensible storage.
Program odczytuje poprzednie wymiary i wskazuje poprzednia pozycje.

3. PipeFlowTagger
Program analizuje przebieg pionow i wstawia odpowiedni symbol w zaleznosci od tego czy pion przechodzi przez cala kondygnacje czy tez przychodzi z gory lub z dolu.

4. PipesInWallSearch
Program analizuje geometrie scian 
z LinkedRVT i sprawdza w projekcie glownym,
ktore odcinki 
rur sa czesciowo lub w calosci w scianie. 
Sprawdza takze czy dany odcinek rury po 
wyjsciu ze sciany laczy sie bezposrednio 
z jakims urzadzeniem. Program ma na celu 
okreslenie czy kolizja ma generowac otwor 
czy nie. Np. rura bedaca w calosci
w scianie GK dla revita wskazuje na kolizje.
W rzeczywistosci rura znajduje sie w przestrzeni
pomiedzy plytami i otwor nie jest potrzebny.
Podobnie rury bedace przylaczem do urzadzenia nie sa brane
pod uwage w otworowaniu.

5. RaumbuchExport
Otrzymalismy od klienta plik excel, w ktorym kazdy arkusz byl przypisany do kazdego pomieszczenia w projekcie.
Zadanie polegalo na wyeksportowaniu do poszczegolnych arkuszu liczby urzadzen okreslonego typu wystepujacych w danym pimieszczeniu.
