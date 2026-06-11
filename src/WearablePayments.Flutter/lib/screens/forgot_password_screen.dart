import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';
import '../services/auth_service.dart';
import '../widgets/loading_button.dart';

class ForgotPasswordScreen extends StatefulWidget {
  const ForgotPasswordScreen({super.key});

  @override
  State<ForgotPasswordScreen> createState() => _ForgotPasswordScreenState();
}

class _ForgotPasswordScreenState extends State<ForgotPasswordScreen> {
  final _email = TextEditingController();
  String _message = '';
  bool _success = false;
  bool _loading = false;

  Future<void> _send() async {
    setState(() { _message = ''; _loading = true; });
    try {
      await context.read<AuthService>().forgotPassword(_email.text.trim());
      setState(() { _success = true; _message = 'If that email is registered, a reset link has been sent.'; });
    } catch (_) {
      setState(() { _success = true; _message = 'If that email is registered, a reset link has been sent.'; });
    } finally {
      if (mounted) setState(() => _loading = false);
    }
  }

  @override
  Widget build(BuildContext context) => Scaffold(
        appBar: AppBar(title: const Text('Reset Password')),
        body: Center(
          child: ConstrainedBox(
            constraints: const BoxConstraints(maxWidth: 400),
            child: Padding(
              padding: const EdgeInsets.all(32),
              child: Column(
                mainAxisSize: MainAxisSize.min,
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Text('Reset Password', style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.bold), textAlign: TextAlign.center),
                  const SizedBox(height: 8),
                  const Text("Enter your email and we'll send a reset link.", textAlign: TextAlign.center, style: TextStyle(color: Colors.grey)),
                  const SizedBox(height: 32),
                  TextField(controller: _email, decoration: const InputDecoration(labelText: 'Email'), keyboardType: TextInputType.emailAddress),
                  if (_message.isNotEmpty) ...[
                    const SizedBox(height: 12),
                    Text(_message, style: TextStyle(color: _success ? Colors.green : Colors.red), textAlign: TextAlign.center),
                  ],
                  const SizedBox(height: 24),
                  LoadingButton(label: 'Send Reset Link', loading: _loading, onPressed: _send),
                  TextButton(onPressed: () => context.pop(), child: const Text('Back to Sign In')),
                ],
              ),
            ),
          ),
        ),
      );
}
