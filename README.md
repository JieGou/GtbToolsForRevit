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
Program analizuje geometrie scian z LinkedRVT i sprawdza, ktore odcinki rur sa czesciowo lub w calosci w scianie. Sprawdza takze czy dany odcinek rury po wyjsciu ze sciany laczy sie bezposrednio z jakims urzadzeniem. Program ma na celu okreslona kolizja ma generowac orwor czy nie. Np. rura bedaca w calosci w scianie GK dla revita generuje kolizje. W rzeczywistosci rura znajduje sie w przestrzeni pomiedzy plytami. 

5.
