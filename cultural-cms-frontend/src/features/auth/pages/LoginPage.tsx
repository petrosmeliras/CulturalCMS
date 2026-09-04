import { useAuth } from "@/features/auth";
import { useNavigate } from "react-router";
import { useForm } from "react-hook-form";
import {type LoginFields, loginSchema} from "@/features/auth/schemas/auth";
import { zodResolver } from "@hookform/resolvers/zod";
import { toast } from "sonner";
import { Box, Button, Container, Paper, TextField, Typography } from "@mui/material";
import t from "../../../locales/el";

export default function LoginPage() {
  const { loginUser } = useAuth();
  const navigate = useNavigate();

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<LoginFields>({
    resolver: zodResolver(loginSchema),
  });

  const onSubmit = async (data: LoginFields) => {
    try {
      await loginUser(data);
      toast.success(t.auth.loginSuccess);
      navigate("/");
    } catch (error) {
      console.error(error);
      toast.error(t.auth.loginError);
    }
  };

  return (
    <Container component="main" maxWidth="xs">
      <Paper
        sx={{
          mt: 8,
          p: 4,
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
        }}
      >
        <Typography component="h1" variant="h5" sx={{ mb: 3, fontWeight: "bold" }}>
          {t.auth.loginButton}
        </Typography>

        <Box component="form" onSubmit={handleSubmit(onSubmit)} sx={{ width: "100%" }}>
          <TextField
            margin="normal"
            fullWidth
            id="username"
            label={t.field.username}
            autoFocus
            {...register("username")}
            error={!!errors.username}
            helperText={errors.username?.message}
          />

          <TextField
            margin="normal"
            fullWidth
            type="password"
            id="password"
            label={t.field.password}
            {...register("password")}
            error={!!errors.password}
            helperText={errors.password?.message}
          />

          <Button
            type="submit"
            fullWidth
            variant="contained"
            sx={{ mt: 4, mb: 2, py: 1.5, fontSize: "1rem" }}
            disabled={isSubmitting}
          >
            {isSubmitting ? t.auth.loginLoading : t.auth.loginButton}
          </Button>

          <Box sx={{ textAlign: "center", mt: 2 }}>
            <Typography variant="body2" color="text.secondary">
              {t.auth.noAccountYet}{" "}
              <Button
                variant="text"
                size="small"
                onClick={() => navigate("/signup")}
                sx={{ textTransform: "none", fontWeight: "bold" }}
              >
                {t.auth.signUpHere}
              </Button>
            </Typography>
          </Box>
        </Box>
      </Paper>
    </Container>
  );
}