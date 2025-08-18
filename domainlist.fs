

: .domain-list ( domlst0 -- )
    drop
    cr ." TODO" cr  
;

: domain-list-deallocate ( domlst0 -- )
     [ ' domain-deallocate ] literal over list-apply    \ Deallocate domain instances in the list.
    list-deallocate                                     \ Deallocate list and links.
;




