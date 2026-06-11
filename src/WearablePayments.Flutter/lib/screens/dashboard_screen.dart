import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';
import '../services/api_service.dart';

class DashboardScreen extends StatefulWidget {
  const DashboardScreen({super.key});

  @override
  State<DashboardScreen> createState() => _DashboardScreenState();
}

class _DashboardScreenState extends State<DashboardScreen> {
  List<dynamic> _transactions = [];
  bool _loading = false;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() => _loading = true);
    try {
      final txs = await context.read<ApiService>().getList('api/transactions');
      setState(() => _transactions = txs);
    } catch (_) {}
    finally { if (mounted) setState(() => _loading = false); }
  }

  Color _statusColor(String status) => switch (status) {
        'Approved' => Colors.green,
        'Declined' => Colors.red,
        'Reversed' => Colors.orange,
        _ => Colors.grey,
      };

  @override
  Widget build(BuildContext context) => Scaffold(
        appBar: AppBar(
          title: const Text('Transactions'),
          actions: [
            IconButton(icon: const Icon(Icons.person_outline), onPressed: () => context.push('/profile')),
          ],
        ),
        body: RefreshIndicator(
          onRefresh: _load,
          child: _loading
              ? const Center(child: CircularProgressIndicator())
              : _transactions.isEmpty
                  ? const Center(child: Text('No transactions yet.'))
                  : ListView.separated(
                      itemCount: _transactions.length,
                      separatorBuilder: (_, __) => const Divider(height: 1),
                      itemBuilder: (_, i) {
                        final tx = _transactions[i];
                        final amount = (tx['amount'] as num).toDouble();
                        final currency = tx['currency'] ?? 'AUD';
                        final status = tx['status'] ?? '';
                        return ListTile(
                          title: Text(tx['merchantName'] ?? '', style: const TextStyle(fontWeight: FontWeight.w600)),
                          subtitle: Text(tx['createdAt']?.toString().substring(0, 16) ?? ''),
                          trailing: Column(
                            mainAxisAlignment: MainAxisAlignment.center,
                            crossAxisAlignment: CrossAxisAlignment.end,
                            children: [
                              Text('\$$amount $currency', style: const TextStyle(fontWeight: FontWeight.bold)),
                              Text(status, style: TextStyle(fontSize: 12, color: _statusColor(status))),
                            ],
                          ),
                        );
                      },
                    ),
        ),
      );
}
