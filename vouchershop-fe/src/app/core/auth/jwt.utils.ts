// src/app/core/auth/jwt.utils.ts
export type JwtPayload = Record<string, any>;

function base64UrlDecode(input: string): string {
  let base64 = input.replace(/-/g, "+").replace(/_/g, "/");
  const pad = base64.length % 4;
  if (pad) base64 += "=".repeat(4 - pad);

  return decodeURIComponent(
    atob(base64)
      .split("")
      .map(c => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2))
      .join("")
  );
}

export function parseJwt(token: string): JwtPayload | null {
  try {
    const [, payload] = token.split(".");
    if (!payload) return null;
    return JSON.parse(base64UrlDecode(payload));
  } catch {
    return null;
  }
}

export function getJwtRoles(payload: JwtPayload | null): string[] {
  if (!payload) return [];

  // ASP.NET Identity tipicamente usa uno di questi:
  const roleKeys = [
    "role",
    "roles",
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
  ];

  for (const key of roleKeys) {
    const value = payload[key];
    if (!value) continue;

    if (Array.isArray(value)) return value.map(String);
    return [String(value)];
  }

  return [];
}
