\ Support the indirect use of functions, due to a function being
\ referenced before being defined.
\
\ Source code reordering can sometimes solve this, but it is not always easy, or aesthetic, and can change with new code.
\
\ So a place holder is defined here, like: ' abort value <function-name>-xt
\
\ In a file before a function is defined, use a line like: "<function-name>-xt execute" instead of "<function-name>"
\
\ After the function is defined, add a line like: ' <function-name> to <function-name>-xt
\

' abort value domain-all-bits-xt

' abort value domain-ms-bit-xt

' abort value domain-max-region-xt


' abort value region-new-xt

' abort value region-subtract-state-xt

' abort value region-deallocate-xt

' abort value region-list-set-union-xt

' abort value region-list-deallocate-xt

' abort value region-list-push-xt







