import { useNavigate } from "react-router";
import { Container, Box, Typography, TextField, Button, Paper } from "@mui/material";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { toast } from "sonner";
import { registerUser } from "../api/auth";
import { signupSchema, type UserSignupFormData } from "../schemas/auth";
import t from "../../../locales/el";
import {isAxiosError} from "axios";

export default function SignupPage() {
  const navigate = useNavigate();

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<UserSignupFormData>({
    resolver: zodResolver(signupSchema),
  });

  const onSubmit = async (data: UserSignupFormData) => {
    try {
      await registerUser(data);
      toast.success(t.auth.signupSuccess);
      navigate("/login");
    } catch (error) {
      console.error(error);
      if (isAxiosError(error) && error.response?.status === 409) {
        toast.error(t.auth.signupConflict);
      } else {
        toast.error(t.auth.signupError);
      }
    }
  };

  return (
    <Container component="main" maxWidth="xs">
      <Paper sx={{ mt: 8, p: 4, display: "flex", flexDirection: "column", alignItems: "center" }}>
        <Typography component="h1" variant="h5" sx={{ mb: 3, fontWeight: "bold" }}>
          {t.auth.signupTitle}
        </Typography>
        <Box component="form" onSubmit={handleSubmit(onSubmit)} sx={{ width: "100%" }}>
          <TextField margin="normal" fullWidth label={t.field.username} {...register("username")} error={!!errors.username} helperText={errors.username?.message} />
          <TextField margin="normal" fullWidth label={t.field.email} type="email" {...register("email")} error={!!errors.email} helperText={errors.email?.message} />
          <TextField margin="normal" fullWidth label={t.field.firstname} {...register("firstname")} error={!!errors.firstname} helperText={errors.firstname?.message} />
          <TextField margin="normal" fullWidth label={t.field.lastname} {...register("lastname")} error={!!errors.lastname} helperText={errors.lastname?.message} />
          <TextField margin="normal" fullWidth label={t.field.password} type="password" {...register("password")} error={!!errors.password} helperText={errors.password?.message} />
          <Button type="submit" fullWidth variant="contained" sx={{ mt: 3, mb: 2 }} disabled={isSubmitting}>
            {isSubmitting ? t.auth.signupLoading : t.auth.signupButton}
          </Button>
          <Box sx={{ textAlign: "center", mt: 2 }}>
            <Typography variant="body2" color="text.secondary">
              {t.auth.alreadyHaveAccount}{" "}
              <Button
                variant="text"
                size="small"
                onClick={() => navigate("/login")}
                sx={{ textTransform: "none", fontWeight: "bold" }}
              >
                {t.auth.loginHere}
              </Button>
            </Typography>
          </Box>
        </Box>
      </Paper>
    </Container>
  );
}