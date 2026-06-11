import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';
import '../services/api_service.dart';
import '../services/auth_service.dart';
import '../widgets/loading_button.dart';

class ProfileScreen extends StatefulWidget {
  const ProfileScreen({super.key});

  @override
  State<ProfileScreen> createState() => _ProfileScreenState();
}

class _ProfileScreenState extends State<ProfileScreen> {
  late final TextEditingController _name;
  final _currentPw = TextEditingController();
  final _newPw = TextEditingController();
  final _confirmPw = TextEditingController();
  String _statusMessage = '';
  bool _isSuccess = false;
  bool _loading = false;

  @override
  void initState() {
    super.initState();
    _name = TextEditingController(text: context.read<AuthService>().fullName);
    _loadProfile();
  }

  Future<void> _loadProfile() async {
    try {
      final p = await context.read<ApiService>().get('api/profile');
      final auth = context.read<AuthService>();
      auth.setProfile(p['fullName'], p['email']);
      setState(() => _name.text = p['fullName']);
    } catch (_) {}
  }

  Future<void> _updateName() async {
    setState(() { _statusMessage = ''; _loading = true; });
    try {
      await context.read<ApiService>().post('api/profile/update-name', {'fullName': _name.text.trim()});
      context.read<AuthService>().setProfile(_name.text.trim(), context.read<AuthService>().email);
      setState(() { _isSuccess = true; _statusMessage = 'Name updated successfully.'; });
    } catch (e) {
      setState(() { _isSuccess = false; _statusMessage = e.toString().replaceFirst('Exception: ', ''); });
    } finally { if (mounted) setState(() => _loading = false); }
  }

  Future<void> _changePassword() async {
    setState(() => _statusMessage = '');
    if (_newPw.text != _confirmPw.text) {
      setState(() { _isSuccess = false; _statusMessage = 'New passwords do not match.'; });
      return;
    }
    if (_newPw.text.length < 8) {
      setState(() { _isSuccess = false; _statusMessage = 'Password must be at least 8 characters.'; });
      return;
    }
    setState(() => _loading = true);
    try {
      await context.read<ApiService>().post('api/profile/change-password', {
        'currentPassword': _currentPw.text,
        'newPassword': _newPw.text,
      });
      _currentPw.clear(); _newPw.clear(); _confirmPw.clear();
      setState(() { _isSuccess = true; _statusMessage = 'Password changed successfully.'; });
    } catch (e) {
      setState(() { _isSuccess = false; _statusMessage = e.toString().replaceFirst('Exception: ', ''); });
    } finally { if (mounted) setState(() => _loading = false); }
  }

  @override
  Widget build(BuildContext context) {
    final auth = context.watch<AuthService>();
    return Scaffold(
      appBar: AppBar(title: const Text('Profile')),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(24),
        child: Center(
          child: ConstrainedBox(
            constraints: const BoxConstraints(maxWidth: 480),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                // Avatar
                Center(
                  child: CircleAvatar(
                    radius: 40,
                    backgroundColor: const Color(0xFF0066CC),
                    child: Text(auth.initials, style: const TextStyle(color: Colors.white, fontSize: 24, fontWeight: FontWeight.bold)),
                  ),
                ),
                const SizedBox(height: 12),
                Text(auth.fullName, textAlign: TextAlign.center, style: const TextStyle(fontSize: 20, fontWeight: FontWeight.bold)),
                Text(auth.email, textAlign: TextAlign.center, style: const TextStyle(color: Colors.grey)),
                const Divider(height: 40),

                // Update name
                const Text('Full Name', style: TextStyle(fontWeight: FontWeight.bold)),
                const SizedBox(height: 8),
                TextField(controller: _name, decoration: const InputDecoration(labelText: 'Full Name')),
                const SizedBox(height: 12),
                LoadingButton(label: 'Update Name', loading: _loading, onPressed: _updateName),
                const Divider(height: 40),

                // Change password
                const Text('Change Password', style: TextStyle(fontWeight: FontWeight.bold)),
                const SizedBox(height: 8),
                TextField(controller: _currentPw, decoration: const InputDecoration(labelText: 'Current Password'), obscureText: true),
                const SizedBox(height: 12),
                TextField(controller: _newPw, decoration: const InputDecoration(labelText: 'New Password'), obscureText: true),
                const SizedBox(height: 12),
                TextField(controller: _confirmPw, decoration: const InputDecoration(labelText: 'Confirm New Password'), obscureText: true),
                const SizedBox(height: 12),
                LoadingButton(label: 'Change Password', loading: _loading, onPressed: _changePassword),

                if (_statusMessage.isNotEmpty) ...[
                  const SizedBox(height: 12),
                  Text(_statusMessage, textAlign: TextAlign.center, style: TextStyle(color: _isSuccess ? Colors.green : Colors.red)),
                ],

                const Divider(height: 40),

                // Sign out
                ElevatedButton(
                  onPressed: () { context.read<AuthService>().signOut(); context.go('/login'); },
                  style: ElevatedButton.styleFrom(backgroundColor: Colors.red, foregroundColor: Colors.white, minimumSize: const Size.fromHeight(48)),
                  child: const Text('Sign Out'),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
