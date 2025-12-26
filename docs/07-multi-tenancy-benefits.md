# Perché l'architettura multi-tenant è la scelta vincente per VoucherShop

L'adozione di un'architettura multi-tenant consente di servire più negozi (**shop**) con un'unica piattaforma mantenendo isolamento logico e operativo. Ecco i motivi per cui è la strategia giusta per VoucherShop.

## Isolamento e sicurezza
- **Dati segregati per `ShopId`**: ogni entità (ad es. `Voucher`, `AppUser`, log di audit) porta il riferimento allo shop, e tutte le query applicano un filtro sul `ShopId`. Questo evita fughe di dati tra negozi e rende più semplice dimostrare conformità (es. GDPR).
- **Autorizzazione contestuale**: il claim `shop_id` e l'implementazione di `ICurrentUser` forniscono ai casi d'uso il contesto del tenant senza esporre dettagli di infrastruttura. Gli handler rifiutano le richieste prive di shop, riducendo la superficie di attacco.

## Scalabilità e operatività
- **Scalare per tenant**: la separazione logica consente di allocare risorse diverse a shop con carichi differenti (database shard o pool separati, throughput API differenziato). L'architettura rimane valida sia con pochi shop sia con centinaia.
- **Evoluzione controllata**: feature flag e rollout graduali possono essere applicati per singolo shop, riducendo il rischio durante i rilasci e facilitando roll-back mirati.

## Manutenibilità e osservabilità
- **Layering pulito**: la Clean Architecture adottata (Domain/Application/Infrastructure/Api) mantiene il core indipendente dai dettagli tecnici; il multi-tenant è modellato nel dominio e propagato tramite interfacce, rendendo i casi d'uso testabili e sostituibili.
- **Auditing per tenant**: i log di audit includono `ShopId`, permettendo a supporto e compliance di ricostruire la storia di ogni voucher per shop. Questo semplifica diagnostica, dispute e analisi forensi mirate.
- **Metriche e throttling dedicati**: con il contesto `ShopId` disponibile a runtime, è semplice aggiungere middleware o pipeline behavior per logging, rate limiting e allarmi specifici per shop.

## Esperienza utente e time-to-market
- **Configurazioni differenziate**: politica di scadenza, valute o campagne promozionali possono essere abilitate per singolo shop senza mantenere istanze separate dell'applicazione.
- **Onboarding rapido**: la creazione di un nuovo shop richiede solo l'inserimento dei dati (e dell'utente admin), senza duplicare infrastruttura. Ciò riduce il costo operativo e accelera l'acquisizione di nuovi clienti.

## Conclusione
Un'architettura multi-tenant permette a VoucherShop di crescere mantenendo sicurezza, controllo operativo e flessibilità commerciale. L'integrazione del `ShopId` a livello di dominio e la separazione in layer applicativi garantiscono che i benefici arrivino senza appesantire il codice o vincolare l'evoluzione futura.
