import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../auth.service';

@Component({
    selector: 'app-register',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, RouterModule],
    templateUrl: './register.component.html',
    styleUrl: './register.component.css'
})
export class RegisterComponent {
    registerForm: FormGroup;
    loading = false;
    errorMessage = '';
    successMessage = '';

    constructor(
        private fb: FormBuilder,
        private authService: AuthService,
        private router: Router
    ) {
        this.registerForm = this.fb.group({
            name: ['', [Validators.required, Validators.minLength(2)]],
            email: ['', [Validators.required, Validators.email]],
            password: ['', [Validators.required, Validators.minLength(6)]]
        });
    }

    onSubmit() {
        if (this.registerForm.valid) {
            this.loading = true;
            this.errorMessage = '';
            this.successMessage = '';

            this.authService.register(this.registerForm.value).subscribe({
                next: (response) => {
                    this.successMessage = 'Usuario registrado exitosamente. Redirigiendo al login...';
                    setTimeout(() => {
                        this.router.navigate(['/login']);
                    }, 2000);
                },
                error: (error) => {
                    this.errorMessage = error.error?.message || 'Error en registro';
                    this.loading = false;
                }
            });
        }
    }
}