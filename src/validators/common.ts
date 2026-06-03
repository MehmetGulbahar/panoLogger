import { z } from 'zod';

export const uuidSchema = z.string().uuid();
export const optionalTrimmedStringSchema = z.string().trim().min(1).optional();