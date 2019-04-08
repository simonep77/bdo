# BDO
BDO - Busssiness Data Objects

BDO è un framework per piattaforma .NET scritto in C# per la mappatura (ORM) dei dati ed il loro utilizzo tramite business objects.
Attualmente supporta i dbms Mysql e SQL Server.

Focus principale è limitare l'uso di query ripetitive e di ottimizzare l'accesso ai dati attraverso caching di dati condivisi, caching in memoria dei dati già caricati per sessione di lavoro.

Essendo ideato prima del linq e come target framework 2.0 ha un suo meta-linguaggio tipo linq per poter eseguire le operazioni più comuni (Sum, Max, Sort, Group By).

Il framework è production ready anche se non ha un progetto di test esaustivo e poca documentazione. All'interno della società dove lavoro è però molto diffuso ed utilizzato.

Il motivo per utilizzarlo al posto di quanto offerto dal mercato? La possibilità di personalizzazione ed il focus all'utilizzo in ambienti multi-cliente, multi-db con un motore in grado di mappare gli oggetti in modalità cross.

Simone
