import 'dart:convert';
import 'package:http/http.dart' as http;

class ApiService {
  static const _base = 'http://localhost:5100';
  String _token = '';

  void setToken(String token) => _token = token;
  String get token => _token;

  Map<String, String> get _headers => {
        'Content-Type': 'application/json',
        if (_token.isNotEmpty) 'Authorization': 'Bearer $_token',
      };

  Future<Map<String, dynamic>> get(String path) async {
    final res = await http.get(Uri.parse('$_base/$path'), headers: _headers);
    _check(res);
    return jsonDecode(res.body);
  }

  Future<List<dynamic>> getList(String path) async {
    final res = await http.get(Uri.parse('$_base/$path'), headers: _headers);
    _check(res);
    return jsonDecode(res.body);
  }

  Future<Map<String, dynamic>> post(String path, Map<String, dynamic> body) async {
    final res = await http.post(
      Uri.parse('$_base/$path'),
      headers: _headers,
      body: jsonEncode(body),
    );
    _check(res);
    return jsonDecode(res.body);
  }

  Future<void> delete(String path) async {
    final res = await http.delete(Uri.parse('$_base/$path'), headers: _headers);
    _check(res);
  }

  void _check(http.Response res) {
    if (res.statusCode >= 400) {
      final body = jsonDecode(res.body);
      throw Exception(body['message'] ?? 'Request failed (${res.statusCode})');
    }
  }
}
