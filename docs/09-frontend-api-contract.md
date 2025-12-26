# Contratto API per il frontend

Questa guida elenca tutte le rotte disponibili, i payload richiesti e le risposte attese così che un team frontend possa implementare client e integrazioni senza ambiguità.

## Principi generali
- **Autenticazione**: le richieste protette usano bearer token JWT nell'header `Authorization: Bearer <accessToken>`; l'access token si ottiene da `/api/auth/login` o `/api/auth/refresh`.
- **Refresh token**: viene emesso come cookie `refreshToken` HttpOnly, `Secure`, `SameSite=None` e con scadenza a 7 giorni; va inviato automaticamente dal browser per `refresh` e `logout`.
- **Multi-tenancy**: il contesto dello shop è codificato nel token tramite la claim `shop_id`; l'API rifiuta azioni tenant-cross (es. creare utenti fuori dallo shop chiamante).
- **Ruoli**: gli Admin possono gestire utenti e voucher; gli utenti autenticati con ruolo User possono leggere solo il proprio voucher.
- **Status code**: indicati per ogni endpoint; le risposte `400/401/403/404` espongono messaggi di errore testuali o gli errori Identity.

## Auth
### POST `/api/auth/register-shop`
- **Uso**: bootstrap iniziale; crea un nuovo Shop e l'utente Admin associato.
- **Body**: `{ "shopName": string, "currency": string, "email": string, "password": string }`.
- **Risposta**: `200 OK` con `{ shopId, adminUserId, adminEmail }`.

### POST `/api/auth/login`
- **Uso**: autenticazione con email e password.
- **Body**: `{ "email": string, "password": string }`.
- **Risposta**: `200 OK` con `{ accessToken }` e set del cookie `refreshToken` HttpOnly.
- **Errori**: `401` se le credenziali non sono valide.

### POST `/api/auth/refresh`
- **Uso**: ruota il refresh token presente nel cookie, revoca il precedente e restituisce un nuovo access token.
- **Body**: nessuno; richiede il cookie `refreshToken` valido.
- **Risposta**: `200 OK` con `{ accessToken }` e nuovo cookie `refreshToken`.
- **Errori**: `401` se il cookie manca, è scaduto o revocato.

### POST `/api/auth/logout`
- **Uso**: revoca il refresh token corrente ed elimina il cookie.
- **Body**: nessuno; richiede il cookie `refreshToken` se presente.
- **Risposta**: `204 No Content`.

### POST `/api/auth/self-register`
- **Uso**: registrazione self-service di un utente User su uno shop specifico (senza login).
- **Body**: `{ "shopId": Guid, "email": string, "password": string }`.
- **Risposta**: `201 Created` con `{ userId, email, shopId }`.
- **Errori**: `404` se lo shop non esiste; `400` se l'email è già usata (nel medesimo o altro shop).

### POST `/api/auth/forgot-password`
- **Uso**: richiede il token di reset password per uno shop specifico.
- **Body**: `{ "shopId": Guid, "email": string }`.
- **Risposta**: `204 No Content` (il token viene generato lato server; l'invio email è da implementare separatamente).
- **Errori**: `404` se l'utente non esiste nello shop indicato.

### POST `/api/auth/reset-password`
- **Uso**: resettare la password con token valido, revocando i refresh token attivi.
- **Body**: `{ "shopId": Guid, "email": string, "token": string, "newPassword": string }`.
- **Risposta**: `204 No Content`.
- **Errori**: `404` se l'utente non esiste nello shop; `400` se il token è invalido o la password non rispetta i requisiti.

### POST `/api/auth/register` (Admin)
- **Uso**: crea un nuovo utente User all'interno dello shop dell'Admin autenticato.
- **Body**: `{ "email": string, "password": string }`.
- **Risposta**: `201 Created` con `{ userId, email }`.
- **Errori**: `401` se manca la claim `shop_id`; `400` se l'utente esiste già.

## Admin
Tutte le rotte richiedono ruolo **Admin** e operano solo sullo shop del chiamante.

### POST `/api/admin/users`
- **Uso**: crea un utente User nello shop dell'Admin.
- **Body**: `{ "email": string, "password": string }`.
- **Risposta**: `200 OK` con `{ userId }`.
- **Errori**: `400` se l'utente esiste già; `401/403` per problemi di autenticazione o ruolo.

### PUT `/api/admin/vouchers/{userId}`
- **Uso**: crea o aggiorna il voucher di un utente specifico nello shop corrente.
- **Body**: `{ "newAmount": decimal, "newExpiresAtUtc": string|null }` (`newExpiresAtUtc` ISO 8601).
- **Risposta**: `204 No Content`.
- **Errori**: `404` se l'utente/voucher non esiste nel tenant; `401/403` se non autorizzato.

### GET `/api/admin/vouchers/{userId}/history`
- **Uso**: recupera l'audit trail del voucher dell'utente.
- **Risposta**: `200 OK` con array di `{ action: string, changesJson: string, performedByUserId: Guid|null, performedAt: string }`.
- **Errori**: `401/403` se non autorizzato.

## User
### GET `/api/me/voucher`
- **Uso**: restituisce il voucher dell'utente autenticato (ruolo User o Admin).
- **Risposta**: `200 OK` con `{ amount: number, currency: string, updateAt: string, expiresAt: string, status: "Active"|"Expired" }`.
- **Errori**: `401` se non autenticato; `404` se l'utente non ha un voucher.

## Health check
### GET `/api/health`
- **Uso**: verifica che l'API sia raggiungibile.
- **Risposta**: `200 OK` con stringa descrittiva.
