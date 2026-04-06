## Implemented improvements

- Introduced `IPaymentValidator` to separate payment scheme validation from `PaymentService`
- Added dedicated validator classes for each payment scheme
- Replaced payment scheme branching in `PaymentService` with rule selection by `PaymentScheme`
- Introduced `IAccountDataStoreFactory` to abstract datastore creation
- Added `AccountDataStoreFactory` to select the primary or backup datastore from configuration
- Updated `PaymentService` to depend on `IAccountDataStoreFactory` and a set of `IPaymentValidator` implementations instead of directly handling datastore creation and validation logic

## Further improvements with more time

- Reduce duplication in tests by introducing builders/helpers and using theories where appropriate
- Split tests into separate files by concern, for example `PaymentService`, validators, and datastore factory
- Make `MakePaymentResult` more expressive by using the Results pattern
- Move balance mutation into a method such as `DebitAmount` (out of `PaymentService`)
- Change `MakePaymentRequest` to an immutable record
