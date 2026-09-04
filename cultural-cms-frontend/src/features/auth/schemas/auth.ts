import { z } from "zod";
import t from "@/locales/el";

export const loginSchema = z.object({
  username: z.string()
    .min(2, { message: t.validation.username })
    .max(50, { message: t.validation.username }),
  password: z.string()
    .min(1, { message: t.validation.password }),
});

export const signupSchema = z.object({
  username: z.string()
    .min(2, { message: t.validation.username })
    .max(50, { message: t.validation.username }),
  email: z.string()
    .email({ message: t.validation.email })
    .max(100, { message: t.validation.email }),
  password: z.string()
    .regex(/(?=.*?[A-Z])(?=.*?[a-z])(?=.*?\d)(?=.*?\W)^.{8,25}$/, {
      message: t.validation.password }),
  firstname: z.string()
    .min(2, { message: t.validation.firstname })
    .max(50, { message: t.validation.firstname }),
  lastname: z.string()
    .min(2, { message: t.validation.lastname })
    .max(50, { message: t.validation.lastname }),
});

export type LoginFields = z.infer<typeof loginSchema>;

export type UserSignupFormData = z.infer<typeof signupSchema>;

