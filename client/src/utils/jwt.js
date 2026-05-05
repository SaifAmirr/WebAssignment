const ROLE_CLAIM = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
const ID_CLAIM   = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier';

export function decodeToken(token) {
  try {
    const base64 = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
    return JSON.parse(atob(base64));
  } catch {
    return null;
  }
}

export function getRole(payload) {
  return payload?.[ROLE_CLAIM] ?? payload?.role ?? null;
}

export function getStudentId(payload) {
  const v = payload?.StudentId ?? payload?.studentId ?? null;
  return v !== null ? Number(v) : null;
}

export function getInstructorId(payload) {
  const v = payload?.InstructorId ?? payload?.instructorId ?? null;
  return v !== null ? Number(v) : null;
}

export function getUserId(payload) {
  const v = payload?.[ID_CLAIM] ?? payload?.sub ?? null;
  return v !== null ? Number(v) : null;
}
