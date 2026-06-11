import 'package:flutter/foundation.dart';
import 'api_service.dart';

class AuthService extends ChangeNotifier {
  ApiService _api;
  String? _fullName;
  String? _email;

  AuthService(this._api);

  void update(ApiService api) => _api = api;

  bool get isLoggedIn => _api.token.isNotEmpty;
  String get fullName => _fullName ?? '';
  String get email => _email ?? '';

  String get initials {
    final parts = fullName.trim().split(' ').where((s) => s.isNotEmpty).toList();
    if (parts.isEmpty) return '?';
    return parts.take(2).map((s) => s[0].toUpperCase()).join();
  }

  Future<void> login(String email, String password) async {
    final res = await _api.post('api/auth/login', {'email': email, 'password': password});
    _api.setToken(res['token']);
    _fullName = res['fullName'];
    _email = res['email'];
    notifyListeners();
  }

  Future<void> register(String fullName, String email, String password) async {
    final res = await _api.post('api/auth/register', {
      'email': email,
      'fullName': fullName,
      'password': password,
    });
    _api.setToken(res['token']);
    _fullName = res['fullName'];
    _email = res['email'];
    notifyListeners();
  }

  Future<void> forgotPassword(String email) =>
      _api.post('api/auth/forgot-password', {'email': email});

  void signOut() {
    _api.setToken('');
    _fullName = null;
    _email = null;
    notifyListeners();
  }

  void setProfile(String fullName, String email) {
    _fullName = fullName;
    _email = email;
    notifyListeners();
  }
}
