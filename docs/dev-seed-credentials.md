# Dev-only seed credentials

The Identity module seeds exactly one Admin account via `dotnet run --project Host -- migrate` (see
`backend/Modules/Identity/Identity.Infrastructure/Persistence/IdentitySeeder.cs`),
since Admin has no self-registration path (AGENTS.md section 4).

```
username: admin
email:    admin@gentongku.local
password: Admin#12345
```

Login accepts either `username` or `email` in the same `identifier` field
(see `Identity.Application/Services/AuthService.cs` — `GetByEmailOrUsernameAsync`).

This is a **dev-only** password, committed intentionally for local development
of this portfolio project. Change it (or seed a different one) before any real
deployment.

Password hashing: BCrypt.Net-Next (work factor 11) via
`Identity.Infrastructure/Services/BcryptPasswordHasher.cs`, chosen over a
hand-rolled PBKDF2 implementation for battle-tested salt/version handling.
